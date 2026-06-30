using Katlog.Api.DTOs;
using Katlog.Api.Models;

namespace Katlog.Api.Repositories.Interfaces;

public interface IBrandRepository
{
    Task<List<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand> CreateAsync(Brand brand);
    Task<Brand> UpdateAsync(int id, UpdateBrandDto dto);
    Task DeleteAsync(Brand brand);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name);
}