using katlog_backend.DTOs;
using katlog_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace katlog_backend.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;

    public BrandsController(IBrandService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<BrandResponseDto>>> GetAll()
       
    {
        var brands = await _service.GetAllAsync();
        return Ok(brands);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponseDto>> GetById(int id)
    {
        var brand = await _service.GetByIdAsync(id);
        return Ok(brand);
    }

    [HttpPost]
    public async Task<ActionResult<BrandResponseDto>> Create(
        CreateBrandDto dto)
    {
        var brand = await _service.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = brand.Id },
            brand
        );
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<BrandResponseDto>> Update(
      
        int id,
        UpdateBrandDto dto)
    {
        var brand = await _service.UpdateAsync(id, dto);
        return Ok(brand);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}