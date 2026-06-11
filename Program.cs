using katlog_backend.Data;
using katlog_backend.Extensions;
using katlog_backend.Middleware;
using KatlogAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/katlog.txt",
        rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<KatlogDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
            .GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddRepositories();
builder.Services.AddServices();


var app = builder.Build();

var enabledApiDocumentation= app.Configuration.GetValue<bool>("EnableSwagger");
if (enabledApiDocumentation)
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseErrorHandlingMiddleware();
app.UseRequestLoggingMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();

