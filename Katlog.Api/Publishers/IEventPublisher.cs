// Katlog.Api/Publishers/Interfaces/IEventPublisher.cs
namespace Katlog.Api.Publishers.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, string key, T eventData)
        where T : class;
}