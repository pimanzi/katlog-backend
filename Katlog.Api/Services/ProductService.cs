using Katlog.Api.DTOs;
using Katlog.Api.Enums;
using Katlog.Api.Exceptions;
using Katlog.Api.Models;
using Katlog.Api.Repositories.Interfaces;
using Katlog.Api.Services.Interfaces;

namespace Katlog.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IReadinessService _readinessService;

    public ProductService(
        IProductRepository repository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        IReadinessService readinessService )
    {
        _repository = repository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _readinessService = readinessService;
    }

    public async Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductQueryParameters queryParameters)
    {
        var (items, totalCount) = await _repository.GetAllAsync(queryParameters);
        var dtos = items.Select(MapToResponseDto).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageSize);

        return new PagedResultDto<ProductResponseDto>(
            dtos,
            totalCount,
            queryParameters.PageNumber,
            queryParameters.PageSize,
            totalPages
        );
         
    }

    public async Task<ProductDetailResponseDto> GetByIdAsync(int id)
    {
        return await GetByIdWithDetailsAsync(id);
    }
    
    public async Task<ProductDetailResponseDto> GetByIdWithDetailsAsync(int id)
    {
        var product = await _repository
            .GetByIdWithDetailsAsync(id);

        if (product is null)
            throw new NotFoundException(
                $"Product {id} not found");

        var readiness = _readinessService.Check(product);

        var productAssets = product.Assets
            .Where(a => a.VariantId is null)
            .Select(MapToAssetResponseDto)
            .ToList();

        var variantAssets = product.Assets
            .Where(a => a.VariantId is not null)
            .Select(MapToAssetResponseDto)
            .ToList();

        var variants = product.Variants
            .Select(v => new VariantResponseDto(
                v.Id,
                v.Name,
                v.VariantCode,
                v.Colour,
                v.Size,
                v.Material,
                v.Barcode,
                v.Status.ToString(),
                v.ProductId,
                v.CreatedAt,
                v.UpdatedAt))
            .ToList();

        return new ProductDetailResponseDto(
            product.Id,
            product.Name,
            product.ProductCode,
            product.Description,
            product.Status.ToString(),
            product.Season.ToString(),
            product.TargetMarket,
            product.Brand.Name,
            product.Category.Name,
            product.CreatedAt,
            product.UpdatedAt,
            variants,
            productAssets,
            variantAssets,
            readiness
        );
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        bool codeExists = await _repository
            .ProductCodeExistsAsync(dto.ProductCode);

        if (codeExists)
            throw new ConflictException(
                $"Product code '{dto.ProductCode}' already exists");
        
        bool brandExists = await _brandRepository.ExistsAsync(dto.BrandId);
        if (!brandExists)
            throw new NotFoundException($"Brand {dto.BrandId} not found");
        
        bool categoryExists = await _categoryRepository
            .ExistsAsync(dto.CategoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category {dto.CategoryId} not found");

        var product = new Product
        {
            Name = dto.Name,
            ProductCode = dto.ProductCode,
            Description = dto.Description,
            Season = dto.Season,
            TargetMarket = dto.TargetMarket,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId
        };

        var created = await _repository.CreateAsync(product);
        return MapToResponseDto(created);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        bool exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException($"Product {id} not found");
        
        if (dto.ProductCode is not null)
        {
            bool codeExists = await _repository
                .ProductCodeExistsAsync(dto.ProductCode);
            if (codeExists)
                throw new BadRequestException(
                    $"Product code '{dto.ProductCode}' already exists");
        }
        
        if (dto.BrandId.HasValue)
        {
            bool brandExists = await _brandRepository
                .ExistsAsync(dto.BrandId.Value);
            if (!brandExists)
                throw new NotFoundException(
                    $"Brand {dto.BrandId.Value} not found");
        }
        
        if (dto.CategoryId.HasValue)
        {
            bool categoryExists = await _categoryRepository
                .ExistsAsync(dto.CategoryId.Value);
            if (!categoryExists)
                throw new NotFoundException(
                    $"Category {dto.CategoryId.Value} not found");
        }

        var updated = await _repository.UpdateAsync(id, dto);
        return MapToResponseDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException($"Product {id} not found");

        await _repository.DeleteAsync(product);
    }
    
    public async Task<ProductResponseDto> SubmitForReviewAsync(int id)
    {
        var product = await _repository
            .GetByIdWithDetailsAsync(id);

        if (product is null)
            throw new NotFoundException(
                $"Product {id} not found");

        if (product.Status != ProductStatus.Draft)
            throw new ConflictException(
                $"Only Draft products can be " +
                $"submitted for review. Current " +
                $"status: {product.Status}");
        
        var errors = new List<string>();

        if (string.IsNullOrEmpty(product.Name))
            errors.Add("Product name is required");

        if (string.IsNullOrEmpty(product.Description))
            errors.Add("Product description is required");

        if (string.IsNullOrEmpty(product.ProductCode))
            errors.Add("Product code is required");

        if (!product.Variants.Any())
            errors.Add("Product must have at least one variant");

        if (!product.Assets.Any())
            errors.Add("Product must have at least one asset");

        if (errors.Any())
            throw new BadRequestException(
                string.Join(", ", errors));

        product.Status = ProductStatus.InReview;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateStatusAsync(product);

        return MapToResponseDto(product);
    }
    
    public async Task<ProductResponseDto> PublishAsync(int id)
    {
        var product = await _repository
            .GetByIdWithDetailsAsync(id);

        if (product is null)
            throw new NotFoundException(
                $"Product {id} not found");

        if (product.Status != ProductStatus.InReview &&
            product.Status != ProductStatus.ReadyToPublish)
            throw new ConflictException(
                $"Only InReview or ReadyToPublish products " +
                $"can be published. Current status: {product.Status}");
        
        var readiness = _readinessService.Check(product);

        if (!readiness.IsReady)
            throw new ConflictException(
                $"Product is not ready to publish: " +
                string.Join(", ", readiness.FailReasons));

        product.Status = ProductStatus.Published;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateStatusAsync(product);

        return MapToResponseDto(product);
    }
    
    public async Task<ProductResponseDto> ArchiveAsync(int id)
    {
        var product = await _repository
            .GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException(
                $"Product {id} not found");

        if (product.Status == ProductStatus.Archived)
            throw new ConflictException(
                "Product is already archived");

        product.Status = ProductStatus.Archived;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateStatusAsync(product);

        return MapToResponseDto(product);
    }
    
    public async Task<ReadinessResponseDto> GetReadinessAsync(int id)
    {
        var product = await _repository
            .GetByIdWithDetailsAsync(id);

        if (product is null)
            throw new NotFoundException(
                $"Product {id} not found");

        return _readinessService.Check(product);
    }
    private static ProductResponseDto MapToResponseDto(Product p)
    {
        return new ProductResponseDto(
            p.Id,
            p.Name,
            p.ProductCode,
            p.Description,
            p.Status.ToString(),
            p.Season.ToString(),
            p.TargetMarket,
            p.BrandId,
            p.Brand.Name,
            p.CategoryId,
            p.Category.Name,
            p.CreatedAt,
            p.UpdatedAt
        );
    }

    private static VariantResponseDto MapToVariantResponseDtoo(Variant v)
    {
        return new VariantResponseDto(
            v.Id,
            v.Name,
            v.VariantCode,
            v.Colour,
            v.Size,
            v.Material,
            v.Barcode,
            v.Status.ToString(),
            v.ProductId,
            v.CreatedAt,
            v.UpdatedAt
        );
    }
    
    private static AssetResponseDto MapToAssetResponseDto(Asset a)
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

}