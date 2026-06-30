using Katlog.Api.DTOs;
using Katlog.Api.Enums;
using Katlog.Api.Models;
using Katlog.Api.Services.Interfaces;

namespace Katlog.Api.Services;

public class ReadinessService : IReadinessService
{
    public ReadinessResponseDto Check(Product product)
    {
        var failReasons = new List<string>();
        
        if (string.IsNullOrEmpty(product.Name))
            failReasons.Add("Product name is missing");

        if (string.IsNullOrEmpty(product.Description))
            failReasons.Add("Product description is missing");

        if (string.IsNullOrEmpty(product.ProductCode))
            failReasons.Add("Product code is missing");
        
        var hasApprovedMainImage = product.Assets
            .Any(a =>
                a.AssetType == AssetType.MainImage &&
                a.Status == AssetStatus.Approved);

        if (!hasApprovedMainImage)
            failReasons.Add(
                "Product must have at least one " +
                "approved Main Image");
        
        var hasRejectedAssets = product.Assets
            .Any(a => a.Status == AssetStatus.Rejected);

        if (hasRejectedAssets)
            failReasons.Add(
                "Product has rejected assets");
        var activeVariants = product.Variants
            .Where(v => v.Status == VariantStatus.Active)
            .ToList();

        foreach (var variant in activeVariants)
        {
            var hasApprovedVariantImage = product.Assets
                .Any(a =>
                    a.VariantId == variant.Id &&
                    a.AssetType == AssetType.VariantImage &&
                    a.Status == AssetStatus.Approved);

            if (!hasApprovedVariantImage)
                failReasons.Add(
                    $"Variant '{variant.Name}' " +
                    $"({variant.VariantCode}) must have " +
                    $"at least one approved Variant Image");
        }

        return new ReadinessResponseDto(
            IsReady: failReasons.Count == 0,
            FailReasons: failReasons
        );
    }
}