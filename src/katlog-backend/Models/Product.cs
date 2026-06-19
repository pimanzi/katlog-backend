using katlog_backend.Enums;

namespace katlog_backend.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ProductCode { get; set; }
    public required string Description { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public Season Season { get; set; }
    public string[] TargetMarket { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
    public Brand Brand { get; set; } = null!;
    public Category Category { get; set; } = null!;
    
    public List<Variant> Variants { get; set; } = new();
    public List<Asset> Assets { get; set; } = new();

}

