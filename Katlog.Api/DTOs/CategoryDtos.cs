using System.ComponentModel.DataAnnotations;

namespace Katlog.Api.DTOs;

public record CategoryResponseDto(
    int Id,
    string Name
);

public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
}

public class UpdateCategoryDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
}