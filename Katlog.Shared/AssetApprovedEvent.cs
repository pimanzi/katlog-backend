namespace Katlog.Shared;

public class AssetApprovedEvent : BaseEvent
{
    public AssetApprovedEvent()
    {
        EventType = "AssetApproved";
    }

    public AssetApprovedPayload Payload { get; set; } = null!;
}

public class AssetApprovedPayload
{
    public int AssetId { get; set; }
    public int ProductId { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
}