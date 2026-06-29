using Katlog.Api.Enums;

namespace Katlog.Api.Models;

public class Variant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string VariantCode { get; set; }
    public required string Colour { get; set; }
    public required string Size { get; set; }
    public required string Material { get; set; }
    public string? Barcode { get; set; }
    public VariantStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public List<Asset> Assets { get; set; } = new();
}