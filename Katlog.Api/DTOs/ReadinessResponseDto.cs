namespace Katlog.Api.DTOs;

public record ReadinessResponseDto(
    bool IsReady,
    List<string> FailReasons
);