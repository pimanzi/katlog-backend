using katlog_backend.DTOs;
using katlog_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace katlog_backend.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        return Ok(category);
    }
    [Authorize (Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(
        CreateCategoryDto dto)
    {
        var category = await _service.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            category
        );
    }
    [Authorize (Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(
        int id,
        UpdateCategoryDto dto)
    {
        var category = await _service.UpdateAsync(id, dto);
        return Ok(category);
    }
    [Authorize (Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}