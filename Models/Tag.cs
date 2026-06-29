namespace katlog_backend.Models;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<AssetTag> AssetTags { get; set; } = new();
}