using System.ComponentModel.DataAnnotations;
using Katlog.Api.Enums;

namespace Katlog.Api.DTOs;

public record VariantResponseDto(
    int Id,
    string Name,
    string VariantCode,
    string Colour,
    string Size,
    string Material,
    string? Barcode,
    string Status,
    int ProductId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class CreateVariantDto
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string VariantCode { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Colour { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Size { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Material { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }
}


public class UpdateVariantDto
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? VariantCode { get; set; }

    [MaxLength(50)]
    public string? Colour { get; set; }

    [MaxLength(50)]
    public string? Size { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }

    public VariantStatus? Status { get; set; }
}