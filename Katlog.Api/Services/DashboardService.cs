using Katlog.Api.DTOs;
using Katlog.Api.Enums;
using Katlog.Api.Repositories.Interfaces;
using Katlog.Api.Services.Interfaces;

namespace Katlog.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly IProductRepository _productRepository;
    private readonly IAssetRepository _assetRepository;

    public DashboardService(
        IProductRepository productRepository,
        IAssetRepository assetRepository)
    {
        _productRepository = productRepository;
        _assetRepository = assetRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var totalProducts = await _productRepository.CountAllAsync();

        var draftProducts = await _productRepository
            .CountByStatusAsync(ProductStatus.Draft);

        var readyToPublish = await _productRepository
            .CountByStatusAsync(ProductStatus.ReadyToPublish);

        var publishedProducts = await _productRepository
            .CountByStatusAsync(ProductStatus.Published);

        var pendingReview = await _assetRepository
            .CountByStatusAsync(AssetStatus.PendingReview);

        var rejectedAssets = await _assetRepository
            .CountByStatusAsync(AssetStatus.Rejected);

        var recentAssets = await _assetRepository.GetRecentlyUploadedAsync(10);

        var recentAssetDtos = recentAssets
            .Select(a => new AssetResponseDto(
                a.Id,
                a.OriginalFileName,
                a.FileName,
                a.ContentType,
                a.FileSize,
                a.FileUrl,
                a.AssetType.ToString(),
                a.Status.ToString(),
                a.Title,
                a.Description,
                a.AssetTags.Select(at => at.Tag.Name).ToList(),
                a.UploadedBy,
                a.UploadedAt,
                a.ProductId,
                a.VariantId
            )).ToList();

        return new DashboardSummaryDto(
            totalProducts,
            draftProducts,
            readyToPublish,
            publishedProducts,
            pendingReview,
            rejectedAssets,
            recentAssetDtos
        );
    }
}