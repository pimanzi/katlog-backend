namespace Katlog.Shared;

public class AssetRejectedEvent : BaseEvent
{
    public AssetRejectedEvent()
    {
        EventType = "AssetRejected";
    }

    public AssetRejectedPayload Payload { get; set; } = null!;
}

public class AssetRejectedPayload
{
    public int AssetId { get; set; }
    public int ProductId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public string RejectedBy { get; set; } = string.Empty;
}