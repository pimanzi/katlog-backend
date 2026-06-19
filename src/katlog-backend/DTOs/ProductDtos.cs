using System.ComponentModel.DataAnnotations;
using katlog_backend.Enums;

namespace katlog_backend.DTOs;

public record ProductResponseDto(
    int Id,
    string Name,
    string ProductCode,
    string Description,
    string Status,
    string Season,
    string[] TargetMarket,
    int BrandId,
    string BrandName,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ProductDetailResponseDto(
    int Id,
    string Name,
    string ProductCode,
    string Description,
    string Status,
    string Season,
    string[] TargetMarket,
    int BrandId,
    string BrandName,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List <VariantResponseDto> Variants,
    List <AssetResponseDto> Assets
    
);

public class CreateProductDto
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string ProductCode { get; set; }

    [Required]
    [MaxLength(1000)]
    public required string Description { get; set; }
    
    [Required]
    public Season Season { get; set; }

    [Required]
    [MinLength(1)]
    public required string[] TargetMarket { get; set; }

    [Required]
    public int BrandId { get; set; }

    [Required]
    public int CategoryId { get; set; }
}

public class UpdateProductDto
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? ProductCode { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public ProductStatus? Status { get; set; }

    public Season? Season { get; set; }

    public string[]? TargetMarket { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }
}