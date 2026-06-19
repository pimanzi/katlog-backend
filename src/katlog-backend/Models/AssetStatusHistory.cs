using katlog_backend.Enums;

namespace katlog_backend.Models;

public class AssetStatusHistory
{
    public int Id { get; set; }

    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public AssetStatus PreviousStatus { get; set; }
    public AssetStatus NewStatus { get; set; }

    public required string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? Comment { get; set; }
}