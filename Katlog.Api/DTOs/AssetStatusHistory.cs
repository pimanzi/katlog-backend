namespace Katlog.Api.DTOs;

public record AssetStatusHistoryResponseDto(
    int Id,
    int AssetId,
    string PreviousStatus,
    string NewStatus,
    string ChangedBy,
    DateTime ChangedAt,
    string? Comment
);