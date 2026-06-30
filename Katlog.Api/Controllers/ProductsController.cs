using Katlog.Api.DTOs;
using Katlog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Katlog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll([FromQuery] ProductQueryParameters queryParameters)
    {
        var products = await _service.GetAllAsync(queryParameters);
        return Ok(products);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailResponseDto>> GetById(int id)
    {
        var product = await _service
            .GetByIdWithDetailsAsync(id);
        return Ok(product);
    }
   
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
   
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> Update(
        int id,
        UpdateProductDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        return Ok(product);
    }
   
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
    [HttpPost("{id:int}/submit-for-review")]
    public async Task<ActionResult<ProductResponseDto>> SubmitForReview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var product = await _service.SubmitForReviewAsync(id, userId);
        return Ok(product);
    }

    [HttpPost("{id:int}/publish")]
    public async Task<ActionResult<ProductResponseDto>> Publish(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var product = await _service.PublishAsync(id, userId);
        return Ok(product);
    }

    [HttpPost("{id:int}/archive")]
    public async Task<ActionResult<ProductResponseDto>> Archive(int id)
    {
        var product = await _service.ArchiveAsync(id);
        return Ok(product);
    }

    [HttpGet("{id:int}/readiness")]
    public async Task<ActionResult<ReadinessResponseDto>> GetReadiness(int id)
    {
        var readiness = await _service.GetReadinessAsync(id);
        return Ok(readiness);
    }
}