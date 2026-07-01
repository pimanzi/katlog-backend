using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CloudinaryDotNet;
using FluentValidation;
using FluentValidation.AspNetCore;
using Katlog.Api.Data;
using Katlog.Api.Extensions;
using Katlog.Api.Middleware;
using Katlog.Api.Models;
using Katlog.Api.Services;
using Katlog.Api.Services.Interfaces;
using Katlog.Api.Settings;
using Katlog.Api.Middleware;
using Katlog.Api.Publishers;
using Katlog.Api.Publishers.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/katlog.txt",
        rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Host.UseSerilog();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<KatlogDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
            .GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddRepositories();
builder.Services.AddServices();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var encodedKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
builder.Services.Configure<JwtSettings>(jwtSettings);
var cloudinarySettings = builder.Configuration
    .GetSection("CloudinarySettings")!
    .Get<CloudinarySettings>();
var cloudinaryAccount = new Account(
    cloudinarySettings.CloudName,
    cloudinarySettings.ApiKey,
    cloudinarySettings.ApiSecret
);

var cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

builder.Services
    .AddSingleton<IEventPublisher, KafkaEventPublisher>();

builder.Services
    .AddIdentityCore<AppUser>(options=>
    {   options.Password.RequireDigit = true; 
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<KatlogDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(encodedKey)
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "Unauthorized",
                    message = "You must be logged in to access this resource"
                });

                await context.Response.WriteAsync(result);
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "Forbidden",
                    message = "You do not have permission to access this resource"
                });

                await context.Response.WriteAsync(result);
            }
        };
    }
);
;


var app = builder.Build();

// seeding roles
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<KatlogDbContext>();
await dbContext.Database.MigrateAsync();
var roleManager = scope.ServiceProvider
    .GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser> >();

if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}
if (!await roleManager.RoleExistsAsync("User"))
{
    await roleManager.CreateAsync(new IdentityRole("User"));
}

//seeding admin if not exist 

var adminEmail = app.Configuration["AdminSettings:Email"];
var adminPassword = app.Configuration["AdminSettings:Password"];
var adminFirstName = app.Configuration["AdminSettings:FirstName"];
var adminLastName = app.Configuration["AdminSettings:LastName"];

var existingAdmin = await userManager
    .FindByEmailAsync(adminEmail!);

if (existingAdmin is null)
{
    var user = new AppUser
    {
        UserName = adminEmail,
        Email = adminEmail,
        FirstName = adminFirstName,
        LastName = adminLastName,
    };
    var result= await userManager.CreateAsync(user, adminPassword!);
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
}
var enabledApiDocumentation= app.Configuration.GetValue<bool>("EnableSwagger");
if (enabledApiDocumentation)
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                       ForwardedHeaders.XForwardedProto
});
app.UseErrorHandlingMiddleware();
app.UseRequestLoggingMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();

