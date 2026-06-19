using katlog_backend.DTOs;
using katlog_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace katlog_backend.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;

    public AssetsController(IAssetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AssetResponseDto>>> GetAll(
        [FromQuery] AssetQueryParameters query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(result);
    }

    [HttpGet("{assetId:int}")]
    public async Task<ActionResult<AssetResponseDto>> GetById(int assetId)
    {
        var asset = await _service.GetByIdAsync(assetId);
        return Ok(asset);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<AssetResponseDto>> Upload(
        [FromForm] CreateAssetDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var asset = await _service.CreateAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { assetId = asset.Id },
            asset
        );
    }

    [HttpPost("{assetId:int}/approve")]
    public async Task<ActionResult<AssetResponseDto>> Approve(int assetId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var asset = await _service.ApproveAsync(assetId, userId);
        return Ok(asset);
    }

    [HttpPost("{assetId:int}/reject")]
    public async Task<ActionResult<AssetResponseDto>> Reject(
        int assetId,
        RejectAssetDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var asset = await _service.RejectAsync(assetId, dto, userId);
        return Ok(asset);
    }

    [HttpPost("{assetId:int}/archive")]
    public async Task<ActionResult<AssetResponseDto>> Archive(int assetId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var asset = await _service.ArchiveAsync(assetId, userId);
        return Ok(asset);
    }

    [HttpGet("{assetId:int}/history")]
    public async Task<ActionResult<List<AssetStatusHistoryResponseDto>>> GetHistory(
        int assetId)
    {
        var history = await _service.GetStatusHistoryAsync(assetId);
        return Ok(history);
    }
}