using Katlog.Api.DTOs;

namespace Katlog.Api.Services.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductQueryParameters queryParameters);
    Task<ProductDetailResponseDto> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}