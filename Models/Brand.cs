namespace katlog_backend.Models;

public class Brand
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Product> Products { get; set; } = new();
}