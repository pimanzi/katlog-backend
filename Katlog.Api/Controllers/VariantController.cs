using Katlog.Api.DTOs;
using Katlog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Katlog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products/{productId}/variants")]
public class VariantsController : ControllerBase
{
    private readonly IVariantService _service;

    public VariantsController(IVariantService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<VariantResponseDto>>> GetAllByProduct(
        int productId)
    {
        var variants = await _service.GetAllByProductIdAsync(productId);
        return Ok(variants);
    }

    [HttpGet("{variantId:int}")]
    public async Task<ActionResult<VariantResponseDto>> GetById(
        int productId,
        int variantId)
    {
        var variant = await _service.GetByIdAsync(productId, variantId);
        return Ok(variant);
    }

    [HttpPost]
    public async Task<ActionResult<VariantResponseDto>> Create(
        int productId,
        CreateVariantDto dto)
    {
        var variant = await _service.CreateAsync(productId, dto);
        return CreatedAtAction(
            nameof(GetById),
            new { productId, variantId = variant.Id },
            variant
        );
    }

    [HttpPut("{variantId:int}")]
    public async Task<ActionResult<VariantResponseDto>> Update(
        int productId,
        int variantId,
        UpdateVariantDto dto)
    {
        var variant = await _service.UpdateAsync(productId, variantId, dto);
        return Ok(variant);
    }

    [HttpDelete("{variantId:int}")]
    public async Task<ActionResult> Delete(
        int productId,
        int variantId)
    {
        await _service.DeleteAsync(productId, variantId);
        return NoContent();
    }
}