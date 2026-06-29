using Katlog.Api.Data;
using Katlog.Api.DTOs;
using Katlog.Api.Models;
using Katlog.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Katlog.Api.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly KatlogDbContext _context;

    public AssetRepository(KatlogDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Asset> Items, int TotalCount)> GetAllAsync(AssetQueryParameters query)
    {
        var assetsQuery = _context.Assets
            .AsNoTracking()
            .Include(a => a.AssetTags)
                .ThenInclude(at => at.Tag)
            .AsQueryable();

        if (query.ProductId.HasValue)
            assetsQuery = assetsQuery.Where(a => a.ProductId == query.ProductId);

        if (query.VariantId.HasValue)
            assetsQuery = assetsQuery.Where(a => a.VariantId == query.VariantId);

        if (!string.IsNullOrEmpty(query.AssetType))
            assetsQuery = assetsQuery.Where(a => a.AssetType.ToString() == query.AssetType);

        if (!string.IsNullOrEmpty(query.Status))
            assetsQuery = assetsQuery.Where(a => a.Status.ToString() == query.Status);

        if (!string.IsNullOrEmpty(query.FileName))
            assetsQuery = assetsQuery.Where(a => a.FileName.Contains(query.FileName));

        if (query.Tags is not null && query.Tags.Any())
        {
            foreach (var tagName in query.Tags)
            {
                assetsQuery = assetsQuery.Where(a => a.AssetTags
                    .Any(at => at.Tag.Name == tagName));
            }
        }

        if (query.UploadedAfter.HasValue)
            assetsQuery = assetsQuery.Where(a => a.UploadedAt >= query.UploadedAfter);

        if (query.UploadedBefore.HasValue)
            assetsQuery = assetsQuery.Where(a => a.UploadedAt <= query.UploadedBefore);

        var totalCount = await assetsQuery.CountAsync();

        var items = await assetsQuery
            .OrderByDescending(a => a.UploadedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Asset?> GetByIdAsync(int id)
    {
        return await _context.Assets
            .AsNoTracking()
            .Include(a => a.AssetTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asset> CreateAsync(Asset asset)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset> UpdateStatusAsync(Asset asset)
    {
        var existingAsset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == asset.Id);

        existingAsset!.Status = asset.Status;

        await _context.SaveChangesAsync();
        return existingAsset;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Assets.AnyAsync(a => a.Id == id);
    }

    public async Task<Tag?> FindTagByNameAsync(string name)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task AddAssetTagAsync(AssetTag assetTag)
    {
        _context.AssetTags.Add(assetTag);
        await _context.SaveChangesAsync();
    }

    public async Task<AssetStatusHistory> CreateStatusHistoryAsync(AssetStatusHistory history)
    {
        _context.AssetStatusHistories.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<List<AssetStatusHistory>> GetStatusHistoryAsync(int assetId)
    {
        return await _context.AssetStatusHistories
            .AsNoTracking()
            .Where(h => h.AssetId == assetId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}