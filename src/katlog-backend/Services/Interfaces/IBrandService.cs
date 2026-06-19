using katlog_backend.DTOs;

namespace katlog_backend.Services.Interfaces;

public interface IBrandService
{
    Task<List<BrandResponseDto>> GetAllAsync();
    Task<BrandResponseDto> GetByIdAsync(int id);
    Task<BrandResponseDto> CreateAsync(CreateBrandDto dto);
    Task<BrandResponseDto> UpdateAsync(int id, UpdateBrandDto dto);
    Task DeleteAsync(int id);
}