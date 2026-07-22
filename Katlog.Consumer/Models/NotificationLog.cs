namespace Katlog.Consumer.Models;

public class NotificationLog
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public int ProductId { get; set; }
    public DateTime Timestamp { get; set; }
        = DateTime.UtcNow;
}