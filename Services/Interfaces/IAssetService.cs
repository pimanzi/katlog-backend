using katlog_backend.DTOs;

namespace katlog_backend.Services.Interfaces;

public interface IAssetService
{
    Task<PagedResultDto<AssetResponseDto>> GetAllAsync(AssetQueryParameters query);
    Task<AssetResponseDto> GetByIdAsync(int id);
    Task<AssetResponseDto> CreateAsync(CreateAssetDto dto, string uploadedByUserId);
    Task<AssetResponseDto> ApproveAsync(int id, string changedByUserId);
    Task<AssetResponseDto> RejectAsync(int id, RejectAssetDto dto, string changedByUserId);
    Task<AssetResponseDto> ArchiveAsync(int id, string changedByUserId);
    Task<List<AssetStatusHistoryResponseDto>> GetStatusHistoryAsync(int id);
}