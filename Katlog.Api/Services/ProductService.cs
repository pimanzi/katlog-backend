using Katlog.Api.DTOs;
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

    public ProductService(
        IProductRepository repository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
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
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException($"Product {id} not found");

        return MapToDetailDto(product);
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

    private static ProductDetailResponseDto MapToDetailDto(Product p)
    {
        return new ProductDetailResponseDto(
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
            p.UpdatedAt,
            p.Variants.Select(MapToVariantResponseDtoo).ToList(),
            p.Assets.Select(MapToAssetResponseDto).ToList()
        );
    }
}