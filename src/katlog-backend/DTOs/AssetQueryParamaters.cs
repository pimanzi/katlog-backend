namespace katlog_backend.DTOs;

public class AssetQueryParameters
{
    public int? ProductId { get; set; }
    public int? VariantId { get; set; }
    public string? AssetType { get; set; }
    public string? Status { get; set; }
    public string? FileName { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime? UploadedAfter { get; set; }
    public DateTime? UploadedBefore { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}