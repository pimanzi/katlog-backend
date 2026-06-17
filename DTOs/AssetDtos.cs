using System.ComponentModel.DataAnnotations;
using katlog_backend.Enums;

namespace katlog_backend.DTOs;

public record AssetResponseDto(
    int Id,
    string OriginalFileName,
    string FileName,
    string ContentType,
    long FileSize,
    string FileUrl,
    string AssetType,
    string Status,
    string? Title,
    string? Description,
    List<string> Tags,
    string UploadedBy,
    DateTime UploadedAt,
    int ProductId,
    int? VariantId
);


public class RejectAssetDto
{
    [Required]
    [MaxLength(500)]
    public required string Reason { get; set; }
}


public class CreateAssetDto
{
    [Required]
    public required IFormFile File { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    [Required]
    public AssetType AssetType { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public List<string>? Tags { get; set; }
}