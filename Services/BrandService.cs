using katlog_backend.DTOs;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using katlog_backend.Services.Interfaces;

namespace katlog_backend.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repository;

    public BrandService(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BrandResponseDto>> GetAllAsync()
    {
        var brands = await _repository.GetAllAsync();
        return brands.Select(b => new BrandResponseDto(
            b.Id,
            b.Name
        )).ToList();
    }

    public async Task<BrandResponseDto> GetByIdAsync(int id)
    {
        var brand = await _repository.GetByIdAsync(id);

        if (brand is null)
            throw new NotFoundException($"Brand {id} not found");

        return new BrandResponseDto(brand.Id, brand.Name);
    }

    public async Task<BrandResponseDto> CreateAsync(CreateBrandDto dto)
    {
        bool nameExists = await _repository.NameExistsAsync(dto.Name);

        if (nameExists)
            throw new BadRequestException($"Brand '{dto.Name}' already exists");

        var brand = new Brand
        {
            Name = dto.Name
        };

        var created = await _repository.CreateAsync(brand);
        return new BrandResponseDto(created.Id, created.Name);
    }

    public async Task<BrandResponseDto> UpdateAsync(int id, UpdateBrandDto dto)
    {
        bool exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException($"Brand {id} not found");
        
        if (dto.Name is not null)
        {
            bool nameExists = await _repository.NameExistsAsync(dto.Name);
            if (nameExists)
                throw new ConflictException($"Brand '{dto.Name}' already exists");
        }

        var updated = await _repository.UpdateAsync(id, dto);
        return new BrandResponseDto(updated.Id, updated.Name);
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _repository.GetByIdAsync(id);

        if (brand is null)
            throw new NotFoundException($"Brand {id} not found");

        await _repository.DeleteAsync(brand);
    }
}