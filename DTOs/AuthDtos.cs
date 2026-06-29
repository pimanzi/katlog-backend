using System.ComponentModel.DataAnnotations;

namespace katlog_backend.DTOs;

public class RegisterDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}

public record AuthResponseDto(
    string Token,
    string Email,
   List <string > Roles
);