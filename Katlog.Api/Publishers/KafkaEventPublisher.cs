using Confluent.Kafka;
using Katlog.Api.Publishers.Interfaces;
using Katlog.Api.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Katlog.Api.Publishers;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;

    public KafkaEventPublisher(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            Acks = Acks.All,
            MessageTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<string, string>(config)
            .Build();
    }

    public async Task PublishAsync<T>(
        string topic, string key, T eventData) where T : class
    {
        try
        {
            var message = JsonSerializer.Serialize(eventData);

            await _producer.ProduceAsync(topic,
                new Message<string, string>
                {
                    Key = key,
                    Value = message
                });

            _logger.LogInformation(
                "Published {EventType} to {Topic} with key {Key}",
                eventData.GetType().Name,
                topic,
                key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to publish {EventType} to {Topic}. " +
                "Kafka may be unavailable. Continuing without " +
                "blocking the request.",
                eventData.GetType().Name,
                topic);
        }
    }
}