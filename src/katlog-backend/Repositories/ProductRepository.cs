using katlog_backend.Data;
using katlog_backend.DTOs;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace katlog_backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly KatlogDbContext _context;

    public ProductRepository(KatlogDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Product>,int  TotalCount)> GetAllAsync(ProductQueryParameters queryParameters)
    {
        var productsQuery = _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p=> p.Variants)
            .Include(p=> p.Assets). AsQueryable();
        
        if (!String.IsNullOrEmpty(queryParameters.Brand))
        {
            productsQuery = productsQuery.Where(p => p.Brand.Name == queryParameters.Brand);
        } 
        if (!String.IsNullOrEmpty(queryParameters.Category))
        {
            productsQuery = productsQuery.Where(p => p.Category.Name == queryParameters.Brand);
        } 
        
        if (!String.IsNullOrEmpty(queryParameters.Status))
        {
            productsQuery = productsQuery.Where(p => p.Status.ToString() == queryParameters.Brand);
        } 
        
        var items = await productsQuery
            .OrderByDescending(a => a.CreatedAt)
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();
        
        var totalCount = await productsQuery.CountAsync();
        
        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        await _context.Entry(product)
            .Reference(p => p.Brand)
            .LoadAsync();
        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();

        return product;
    }

    public async Task<Product> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException($"Product {id} not found");

        if (dto.Name is not null)
            product.Name = dto.Name;

        if (dto.ProductCode is not null)
            product.ProductCode = dto.ProductCode;

        if (dto.Description is not null)
            product.Description = dto.Description;

        if (dto.Status.HasValue)
            product.Status = dto.Status.Value;

        if (dto.Season.HasValue)
            product.Season = dto.Season.Value;

        if (dto.TargetMarket is not null)
            product.TargetMarket = dto.TargetMarket;

        if (dto.BrandId.HasValue)
            product.BrandId = dto.BrandId.Value;

        if (dto.CategoryId.HasValue)
            product.CategoryId = dto.CategoryId.Value;

        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ProductCodeExistsAsync(string productCode)
    {
        return await _context.Products
            .AnyAsync(p => p.ProductCode.ToLower() == productCode.ToLower());
    }
}