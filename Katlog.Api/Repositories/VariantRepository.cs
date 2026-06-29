using Katlog.Api.Data;
using Katlog.Api.DTOs;
using Katlog.Api.Exceptions;
using Katlog.Api.Models;
using Katlog.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Katlog.Api.Repositories;

public class VariantRepository : IVariantRepository
{
    private readonly KatlogDbContext _context;

    public VariantRepository(KatlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Variant>> GetAllByProductIdAsync(int productId)
    {
        return await _context.Variants
            .AsNoTracking()
            .Where(v => v.ProductId == productId)
            .ToListAsync();
    }

    public async Task<Variant?> GetByIdAsync(int id)
    {
        return await _context.Variants
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Variant> CreateAsync(Variant variant)
    {
        _context.Variants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<Variant> UpdateAsync(int id, UpdateVariantDto dto)
    {
        var variant = await _context.Variants
            .FirstOrDefaultAsync(v => v.Id == id);

        if (variant is null)
            throw new NotFoundException($"Variant {id} not found");

        if (dto.Name is not null)
            variant.Name = dto.Name;

        if (dto.VariantCode is not null)
            variant.VariantCode = dto.VariantCode;

        if (dto.Colour is not null)
            variant.Colour = dto.Colour;

        if (dto.Size is not null)
            variant.Size = dto.Size;

        if (dto.Material is not null)
            variant.Material = dto.Material;

        if (dto.Barcode is not null)
            variant.Barcode = dto.Barcode;

        if (dto.Status.HasValue)
            variant.Status = dto.Status.Value;

        variant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task DeleteAsync(Variant variant)
    {
        _context.Variants.Remove(variant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Variants
            .AnyAsync(v => v.Id == id);
    }

    public async Task<bool> VariantCodeExistsAsync(int productId, string variantCode)
    {
        return await _context.Variants
            .AnyAsync(v => v.ProductId == productId 
                        && v.VariantCode.ToLower() == variantCode.ToLower());
    }
}