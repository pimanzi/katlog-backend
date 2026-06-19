using katlog_backend.Enums;

namespace katlog_backend.Models;

public class Asset
{
    public int Id { get; set; }

    public required string OriginalFileName { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
    public required string FileUrl { get; set; }

    public AssetType AssetType { get; set; }
    public AssetStatus Status { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public required string UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    
    public Product Product { get; set; } = null!;
    public Variant? Variant { get; set; }

    public List<AssetStatusHistory> StatusHistory { get; set; } = new();
    public List<AssetTag> AssetTags { get; set; } = new();
}