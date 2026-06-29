using Katlog.Api.DTOs;

namespace Katlog.Api.Services.Interfaces;

public interface IAuthService
{
   Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
   Task<AuthResponseDto> LoginAsync(LoginDto dto);

}