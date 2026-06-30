using Katlog.Api.DTOs;

namespace Katlog.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}