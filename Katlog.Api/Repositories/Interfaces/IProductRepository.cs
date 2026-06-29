using Katlog.Api.DTOs;
using Katlog.Api.Models;

namespace Katlog.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(List<Product>,int  TotalCount)> GetAllAsync(ProductQueryParameters queryParameters);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(Product product);
    Task<bool> ExistsAsync(int id);
    Task<bool> ProductCodeExistsAsync(string productCode);
}