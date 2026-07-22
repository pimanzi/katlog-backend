using Confluent.Kafka;
using Katlog.Consumer.Data;
using Katlog.Consumer.Models;
using Katlog.Consumer.Settings;
using Katlog.Shared;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Katlog.Consumer.Workers;

public class AssetEventConsumer : BackgroundService
{
    private readonly ILogger<AssetEventConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<string, string> _consumer;

    public AssetEventConsumer(
        ILogger<AssetEventConsumer> logger,
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var config = new ConsumerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            GroupId = settings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .Build();
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _consumer.Subscribe("catalogue.asset-events");

        _logger.LogInformation(
            "AssetEventConsumer started!");

        await Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer
                        .Consume(stoppingToken);

                    if (result?.Message?.Value is null)
                        continue;

                    await HandleMessage(result.Message.Value);

                    _consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing message, skipping...");
                }
            }
        }, stoppingToken);

        _consumer.Close();
        _logger.LogInformation(
            "AssetEventConsumer stopped gracefully!");
    }

    private async Task HandleMessage(string messageValue)
    {
        var envelope = JsonSerializer
            .Deserialize<EventEnvelope>(messageValue);

        if (envelope is null)
        {
            _logger.LogWarning(
                "Malformed message received, skipping");
            return;
        }

        switch (envelope.EventType)
        {
            case "AssetApproved":
                var approved = JsonSerializer
                    .Deserialize<AssetApprovedEvent>(messageValue);

                if (approved?.Payload is null)
                {
                    _logger.LogWarning(
                        "AssetApproved payload is null, skipping");
                    return;
                }

                await WriteNotificationLog(
                    "AssetApproved",
                    approved.Payload.AssetId,
                    approved.Payload.ProductId);
                break;

            case "AssetRejected":
                var rejected = JsonSerializer
                    .Deserialize<AssetRejectedEvent>(messageValue);

                if (rejected?.Payload is null)
                {
                    _logger.LogWarning(
                        "AssetRejected payload is null, skipping");
                    return;
                }

                await WriteNotificationLog(
                    "AssetRejected",
                    rejected.Payload.AssetId,
                    rejected.Payload.ProductId);
                break;

            default:
                _logger.LogInformation(
                    "Ignoring event type: {EventType}",
                    envelope.EventType);
                break;
        }
    }

    private async Task WriteNotificationLog(
        string eventType, int assetId, int productId)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ConsumerDbContext>();

        var log = new NotificationLog
        {
            EventType = eventType,
            AssetId = assetId,
            ProductId = productId,
            Timestamp = DateTime.UtcNow
        };

        dbContext.NotificationLogs.Add(log);
        await dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "NotificationLog written: {EventType} " +
            "for Asset {AssetId}, Product {ProductId}",
            eventType, assetId, productId);
    }
}