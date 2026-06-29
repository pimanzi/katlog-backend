using Katlog.Api.Data;
using Katlog.Api.DTOs;
using Katlog.Api.Exceptions;
using Katlog.Api.Models;
using Katlog.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Katlog.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly KatlogDbContext _context;

    public CategoryRepository(KatlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            throw new NotFoundException($"Category {id} not found");

        if (dto.Name is not null)
            category.Name = dto.Name;

        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }
}