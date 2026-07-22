using Katlog.Consumer.Data;
using Katlog.Consumer.Settings;
using Katlog.Consumer.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

builder.Services.AddDbContext<ConsumerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
            .GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<AssetEventConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ConsumerDbContext>();

    var pending = await db.Database
        .GetPendingMigrationsAsync();

    if (pending.Any())
    {
        await db.Database.MigrateAsync();
    }
}

await app.RunAsync();