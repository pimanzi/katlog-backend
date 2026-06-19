using katlog_backend.DTOs;
using katlog_backend.Enums;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using katlog_backend.Services.Interfaces;

namespace katlog_backend.Services;

public class VariantService : IVariantService
{
    private readonly IVariantRepository _repository;
    private readonly IProductRepository _productRepository;

    public VariantService(
        IVariantRepository repository,
        IProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<List<VariantResponseDto>> GetAllByProductIdAsync(int productId)
    {
        bool productExists = await _productRepository.ExistsAsync(productId);
        if (!productExists)
            throw new NotFoundException($"Product {productId} not found");

        var variants = await _repository.GetAllByProductIdAsync(productId);
        return variants.Select(MapToResponseDto).ToList();
    }

    public async Task<VariantResponseDto> GetByIdAsync(int productId, int id)
    {
        var variant = await _repository.GetByIdAsync(id);

        if (variant is null || variant.ProductId != productId)
            throw new NotFoundException(
                $"Variant {id} not found for product {productId}");

        return MapToResponseDto(variant);
    }

    public async Task<VariantResponseDto> CreateAsync(int productId, CreateVariantDto dto)
    {
        bool productExists = await _productRepository.ExistsAsync(productId);
        if (!productExists)
            throw new NotFoundException($"Product {productId} not found");

        bool codeExists = await _repository
            .VariantCodeExistsAsync(productId, dto.VariantCode);

        if (codeExists)
            throw new ConflictException(
                $"Variant code '{dto.VariantCode}' already exists for this product");

        var variant = new Variant
        {
            Name = dto.Name,
            VariantCode = dto.VariantCode,
            Colour = dto.Colour,
            Size = dto.Size,
            Material = dto.Material,
            Barcode = dto.Barcode,
            Status = VariantStatus.Active,
            ProductId = productId
        };

        var created = await _repository.CreateAsync(variant);
        return MapToResponseDto(created);
    }

    public async Task<VariantResponseDto> UpdateAsync(int productId, int id, UpdateVariantDto dto)
    {
        var existingVariant = await _repository.GetByIdAsync(id);

        if (existingVariant is null || existingVariant.ProductId != productId)
            throw new NotFoundException(
                $"Variant {id} not found for product {productId}");

        if (dto.VariantCode is not null)
        {
            bool codeExists = await _repository
                .VariantCodeExistsAsync(productId, dto.VariantCode);

            if (codeExists && dto.VariantCode != existingVariant.VariantCode)
                throw new ConflictException(
                    $"Variant code '{dto.VariantCode}' already exists for this product");
        }

        var updated = await _repository.UpdateAsync(id, dto);
        return MapToResponseDto(updated);
    }

    public async Task DeleteAsync(int productId, int id)
    {
        var variant = await _repository.GetByIdAsync(id);

        if (variant is null || variant.ProductId != productId)
            throw new NotFoundException(
                $"Variant {id} not found for product {productId}");

        await _repository.DeleteAsync(variant);
    }

    private static VariantResponseDto MapToResponseDto(Variant v)
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
}