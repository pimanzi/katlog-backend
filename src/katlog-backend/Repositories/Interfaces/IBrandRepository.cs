using katlog_backend.DTOs;
using katlog_backend.Models;

namespace katlog_backend.Repositories.Interfaces;

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