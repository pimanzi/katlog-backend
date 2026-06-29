using Katlog.Api.DTOs;
using Katlog.Api.Models;

namespace Katlog.Api.Repositories.Interfaces;

public interface IAssetRepository
{
    Task<(List<Asset> Items, int TotalCount)> GetAllAsync(AssetQueryParameters query);
    Task<Asset?> GetByIdAsync(int id);
    Task<Asset> CreateAsync(Asset asset);
    Task<Asset> UpdateStatusAsync(Asset asset);
    Task<bool> ExistsAsync(int id);

    Task<Tag?> FindTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(Tag tag);

    Task AddAssetTagAsync(AssetTag assetTag);

    Task<AssetStatusHistory> CreateStatusHistoryAsync(AssetStatusHistory history);
    Task<List<AssetStatusHistory>> GetStatusHistoryAsync(int assetId);
}