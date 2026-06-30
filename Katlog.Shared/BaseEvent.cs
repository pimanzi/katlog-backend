namespace Katlog.Shared;

public abstract class BaseEvent
{
    public string EventId { get; set; }
        = Guid.NewGuid().ToString();

    public string EventType { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; }
        = DateTime.UtcNow;

    public int Version { get; set; } = 1;
}