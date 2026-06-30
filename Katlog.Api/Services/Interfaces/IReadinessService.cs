
using Katlog.Api.DTOs;
using Katlog.Api.Models;

namespace Katlog.Api.Services.Interfaces;

public interface IReadinessService
{
    ReadinessResponseDto Check(Product product);
}