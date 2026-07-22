using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Katlog.Api.Data;
using Katlog.Api.DTOs;
using Katlog.Api.Enums;
using Katlog.Api.Exceptions;
using Katlog.Api.Models;
using Katlog.Api.Publishers.Interfaces;
using Katlog.Api.Repositories.Interfaces;
using Katlog.Api.Services.Interfaces;
using Katlog.Shared;

namespace Katlog.Api.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IVariantRepository _variantRepository;
    private readonly Cloudinary _cloudinary;
    private readonly KatlogDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public AssetService(
        IAssetRepository repository,
        IProductRepository productRepository,
        IVariantRepository variantRepository,
        Cloudinary cloudinary,
        KatlogDbContext context, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _cloudinary = cloudinary;
        _context = context;
        _eventPublisher = eventPublisher;
    }

    private static AssetResponseDto MapToResponseDto(Asset a)
    {
        return new AssetResponseDto(
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
        );
    }

    public async Task<PagedResultDto<AssetResponseDto>> GetAllAsync(AssetQueryParameters query)
    {
        var (items, totalCount) = await _repository.GetAllAsync(query);

        var dtos = items.Select(MapToResponseDto).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResultDto<AssetResponseDto>(
            dtos,
            totalCount,
            query.PageNumber,
            query.PageSize,
            totalPages
        );
    }

    public async Task<AssetResponseDto> GetByIdAsync(int id)
    {
        var asset = await _repository.GetByIdAsync(id);

        if (asset is null)
            throw new NotFoundException($"Asset {id} not found");

        return MapToResponseDto(asset);
    }

    public async Task<AssetResponseDto> CreateAsync(CreateAssetDto dto, string uploadedByUserId)
    {
        bool productExists = await _productRepository.ExistsAsync(dto.ProductId);
        if (!productExists)
            throw new NotFoundException($"Product {dto.ProductId} not found");

        if (dto.VariantId.HasValue)
        {
            var variant = await _variantRepository.GetByIdAsync(dto.VariantId.Value);

            if (variant is null)
                throw new NotFoundException($"Variant {dto.VariantId} not found");

            if (variant.ProductId != dto.ProductId)
                throw new BadRequestException("Variant does not belong to this product");
        }

        var uploadResult = await _cloudinary.UploadAsync(new RawUploadParams
        {
            File = new FileDescription(dto.File.FileName, dto.File.OpenReadStream())
        });

        if (uploadResult.Error is not null)
            throw new BadRequestException($"Upload failed: {uploadResult.Error.Message}");

        var asset = new Asset
        {
            OriginalFileName = dto.File.FileName,
            FileName = uploadResult.PublicId,
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            FileUrl = uploadResult.SecureUrl.ToString(),
            AssetType = dto.AssetType,
            Status = AssetStatus.PendingReview,
            Title = dto.Title,
            Description = dto.Description,
            UploadedBy = uploadedByUserId,
            ProductId = dto.ProductId,
            VariantId = dto.VariantId
        };

        var createdAsset = await _repository.CreateAsync(asset);

        if (dto.Tags is not null && dto.Tags.Any())
        {
            foreach (var tagName in dto.Tags)
            {
                var tag = await _repository.FindTagByNameAsync(tagName);

                if (tag is null)
                    tag = await _repository.CreateTagAsync(new Tag { Name = tagName });

                await _repository.AddAssetTagAsync(new AssetTag
                {
                    AssetId = createdAsset.Id,
                    TagId = tag.Id
                });
            }
        }

        var finalAsset = await _repository.GetByIdAsync(createdAsset.Id);
        
        await _eventPublisher.PublishAsync(
            "catalogue.asset-events",
            finalAsset!.ProductId.ToString(),
            new AssetUploadedEvent()
            {
                Payload = new AssetUploadedPayload
                {
                    AssetId = finalAsset.Id,
                    ProductId = finalAsset.ProductId,
                    VariantId = finalAsset.VariantId,
                    AssetType = finalAsset.AssetType.ToString(),
                    FileName = finalAsset.FileName,
                    UploadedBy = finalAsset.UploadedBy
                }
            });
        return MapToResponseDto(finalAsset!);
    }

    public async Task<AssetResponseDto> ApproveAsync(int id, string changedByUserId)
    {
        var asset = await _repository.GetByIdAsync(id);

        if (asset is null)
            throw new NotFoundException($"Asset {id} not found");

        if (asset.Status != AssetStatus.PendingReview)
            throw new ConflictException(
                "Only assets in PendingReview status can be approved");

        var previousStatus = asset.Status;
        asset.Status = AssetStatus.Approved;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _repository.UpdateStatusAsync(asset);

            await _repository.CreateStatusHistoryAsync(new AssetStatusHistory
            {
                AssetId = asset.Id,
                PreviousStatus = previousStatus,
                NewStatus = AssetStatus.Approved,
                ChangedBy = changedByUserId
            });

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        await _eventPublisher.PublishAsync(
            "catalogue.asset-events",
            asset.ProductId.ToString(),
            new AssetApprovedEvent
            {
                Payload = new AssetApprovedPayload
                {
                    AssetId = asset.Id,
                    ProductId = asset.ProductId,
                    ApprovedBy = changedByUserId
                }
            });

        return MapToResponseDto(asset);
    }

    public async Task<AssetResponseDto> RejectAsync(int id, RejectAssetDto dto, string changedByUserId)
    {
        var asset = await _repository.GetByIdAsync(id);

        if (asset is null)
            throw new NotFoundException($"Asset {id} not found");

        if (asset.Status != AssetStatus.PendingReview)
            throw new ConflictException(
                "Only assets in PendingReview status can be rejected");

        var previousStatus = asset.Status;
        asset.Status = AssetStatus.Rejected;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _repository.UpdateStatusAsync(asset);

            await _repository.CreateStatusHistoryAsync(new AssetStatusHistory
            {
                AssetId = asset.Id,
                PreviousStatus = previousStatus,
                NewStatus = AssetStatus.Rejected,
                ChangedBy = changedByUserId,
                Comment = dto.Reason
            });

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        await _eventPublisher.PublishAsync(
            "catalogue.asset-events",
            asset.ProductId.ToString(),
            new AssetRejectedEvent
            {
                Payload = new AssetRejectedPayload
                {
                    AssetId = asset.Id,
                    ProductId = asset.ProductId,
                    RejectionReason = dto.Reason,
                    RejectedBy = changedByUserId
                }
            });


        return MapToResponseDto(asset);
    }

    public async Task<AssetResponseDto> ArchiveAsync(int id, string changedByUserId)
    {
        var asset = await _repository.GetByIdAsync(id);

        if (asset is null)
            throw new NotFoundException($"Asset {id} not found");

        var previousStatus = asset.Status;
        asset.Status = AssetStatus.Archived;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _repository.UpdateStatusAsync(asset);

            await _repository.CreateStatusHistoryAsync(new AssetStatusHistory
            {
                AssetId = asset.Id,
                PreviousStatus = previousStatus,
                NewStatus = AssetStatus.Archived,
                ChangedBy = changedByUserId
            });

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return MapToResponseDto(asset);
    }

    public async Task<List<AssetStatusHistoryResponseDto>> GetStatusHistoryAsync(int id)
    {
        bool exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException($"Asset {id} not found");

        var history = await _repository.GetStatusHistoryAsync(id);

        return history.Select(h => new AssetStatusHistoryResponseDto(
            h.Id,
            h.AssetId,
            h.PreviousStatus.ToString(),
            h.NewStatus.ToString(),
            h.ChangedBy,
            h.ChangedAt,
            h.Comment
        )).ToList();
    }
}