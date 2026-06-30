namespace Katlog.Api.Enums;

public enum AssetType
{
    MainImage,
    VariantImage,
    LifestyleImage,
    MarketingBanner,
    SizeGuide,
    TechnicalDocument
}

public enum AssetStatus
{
    Uploaded,
    PendingReview,
    Approved,
    Rejected,
    Archived
}