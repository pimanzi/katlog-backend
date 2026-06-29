using Katlog.Api.DTOs;
using Katlog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Katlog.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;

    public BrandsController(IBrandService service)
    {
        _service = service;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<BrandResponseDto>>> GetAll()
       
    {
        var brands = await _service.GetAllAsync();
        return Ok(brands);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponseDto>> GetById(int id)
    {
        var brand = await _service.GetByIdAsync(id);
        return Ok(brand);
    }

    [Authorize (Roles = "Admin")]
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
    [Authorize (Roles = "Admin")]
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<BrandResponseDto>> Update(
      
        int id,
        UpdateBrandDto dto)
    {
        var brand = await _service.UpdateAsync(id, dto);
        return Ok(brand);
    }
    [Authorize (Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}