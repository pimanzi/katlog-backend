
namespace Katlog.Shared;

public class ProductPublishedEvent : BaseEvent
{
    public ProductPublishedEvent()
    {
        EventType = "ProductPublished";
    }

    public ProductPublishedPayload Payload { get; set; } = null!;
}

public class ProductPublishedPayload
{
    public int ProductId { get; set; }
    public string PublishedBy { get; set; } = string.Empty;
}