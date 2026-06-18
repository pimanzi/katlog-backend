using katlog_backend.DTOs;
using katlog_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace katlog_backend.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll([FromQuery] ProductQueryParameters queryParameters)
    {
        var products = await _service.GetAllAsync(queryParameters);
        return Ok(products);
    }
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailResponseDto>> GetById(
        int id)
    {
        var product = await _service.GetByIdAsync(id);
        return Ok(product);
    }
    [Authorize (Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(
        CreateProductDto dto)
    {
        var product = await _service.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product
        );
    }
    [Authorize (Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> Update(
        int id,
        UpdateProductDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        return Ok(product);
    }
    [Authorize (Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}