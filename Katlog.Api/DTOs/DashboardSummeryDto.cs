namespace Katlog.Api.DTOs;

public record DashboardSummaryDto(
    int TotalProducts,
    int DraftProducts,
    int ReadyToPublishProducts,
    int PublishedProducts,
    int AssetsPendingReview,
    int RejectedAssets,
    List<AssetResponseDto> RecentlyUploadedAssets
);