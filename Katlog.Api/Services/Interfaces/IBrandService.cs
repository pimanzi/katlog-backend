using Katlog.Api.DTOs;

namespace Katlog.Api.Services.Interfaces;

public interface IBrandService
{
    Task<List<BrandResponseDto>> GetAllAsync();
    Task<BrandResponseDto> GetByIdAsync(int id);
    Task<BrandResponseDto> CreateAsync(CreateBrandDto dto);
    Task<BrandResponseDto> UpdateAsync(int id, UpdateBrandDto dto);
    Task DeleteAsync(int id);
}