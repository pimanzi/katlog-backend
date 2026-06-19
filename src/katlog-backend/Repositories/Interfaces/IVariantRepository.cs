using katlog_backend.DTOs;
using katlog_backend.Models;

namespace katlog_backend.Repositories.Interfaces;

public interface IVariantRepository
{
    Task<List<Variant>> GetAllByProductIdAsync(int productId);
    Task<Variant?> GetByIdAsync(int id);
    Task<Variant> CreateAsync(Variant variant);
    Task<Variant> UpdateAsync(int id, UpdateVariantDto dto);
    Task DeleteAsync(Variant variant);
    Task<bool> ExistsAsync(int id);
    Task<bool> VariantCodeExistsAsync(int productId, string variantCode);
}