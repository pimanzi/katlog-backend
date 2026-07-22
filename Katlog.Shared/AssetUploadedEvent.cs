namespace Katlog.Shared;

public class AssetUploadedEvent : BaseEvent
{
    public AssetUploadedEvent()
    {
        EventType = "AssetUploaded";
    }

    public AssetUploadedPayload Payload { get; set; } = null!;
}

public class AssetUploadedPayload
{
    public int AssetId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
}