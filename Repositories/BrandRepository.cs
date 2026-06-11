using katlog_backend.Data;
using katlog_backend.DTOs;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace katlog_backend.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly KatlogDbContext _context;

    public BrandRepository(KatlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Brand> CreateAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand> UpdateAsync(int id, UpdateBrandDto dto)
    {
        var brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand is null)
            throw new NotFoundException($"Brand {id} not found");

        if (dto.Name is not null)
            brand.Name = dto.Name;

        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task DeleteAsync(Brand brand)
    {
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Brands
            .AnyAsync(b => b.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Brands
            .AnyAsync(b => b.Name.ToLower() == name.ToLower());
    }
}