using katlog_backend.DTOs;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using katlog_backend.Services.Interfaces;

namespace katlog_backend.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => new CategoryResponseDto(
            c.Id,
            c.Name
        )).ToList();
    }

    public async Task<CategoryResponseDto> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            throw new NotFoundException($"Category {id} not found");

        return new CategoryResponseDto(category.Id, category.Name);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        bool nameExists = await _repository.NameExistsAsync(dto.Name);

        if (nameExists)
            throw new BadRequestException($"Category '{dto.Name}' already exists");

        var category = new Category
        {
            Name = dto.Name
        };

        var created = await _repository.CreateAsync(category);
        return new CategoryResponseDto(created.Id, created.Name);
    }

    public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        bool exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException($"Category {id} not found");

        if (dto.Name is not null)
        {
            bool nameExists = await _repository.NameExistsAsync(dto.Name);
            if (nameExists)
                throw new BadRequestException($"Category '{dto.Name}' already exists");
        }

        var updated = await _repository.UpdateAsync(id, dto);
        return new CategoryResponseDto(updated.Id, updated.Name);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            throw new NotFoundException($"Category {id} not found");

        await _repository.DeleteAsync(category);
    }
}