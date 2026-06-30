using Katlog.Api.Enums;

namespace Katlog.Api.DTOs;

public class ProductQueryParameters
{
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    
    public int PageNumber { get; set; } = 1;
    
    public int PageSize { get; set; } = 20;
    
}