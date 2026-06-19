using katlog_backend.DTOs;

namespace katlog_backend.Services.Interfaces;

public interface IVariantService
{
    Task<List<VariantResponseDto>> GetAllByProductIdAsync(int productId);
    Task<VariantResponseDto> GetByIdAsync(int productId, int id);
    Task<VariantResponseDto> CreateAsync(int productId, CreateVariantDto dto);
    Task<VariantResponseDto> UpdateAsync(int productId, int id, UpdateVariantDto dto);
    Task DeleteAsync(int productId, int id);
}