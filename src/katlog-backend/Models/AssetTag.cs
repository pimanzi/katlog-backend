namespace katlog_backend.Models;

public class AssetTag
{
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}