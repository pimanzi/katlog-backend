using katlog_backend.DTOs;
using katlog_backend.Models;

namespace katlog_backend.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(int id, UpdateCategoryDto dto);
    Task DeleteAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name);
}