using System.ComponentModel.DataAnnotations;

namespace katlog_backend.DTOs;

public record BrandResponseDto(
    int Id,
    string Name
);

public class CreateBrandDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
}

public class UpdateBrandDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
}