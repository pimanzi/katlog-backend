using Katlog.Api.DTOs;

namespace Katlog.Api.Services.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductQueryParameters queryParameters);
    Task<ProductDetailResponseDto> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
    Task<ProductResponseDto> SubmitForReviewAsync(int id);
    Task<ProductResponseDto> PublishAsync(int id);
    Task<ProductResponseDto> ArchiveAsync(int id);
    Task<ReadinessResponseDto> GetReadinessAsync(int id);
    Task<ProductDetailResponseDto> GetByIdWithDetailsAsync(int id);
    Task DeleteAsync(int id);
}