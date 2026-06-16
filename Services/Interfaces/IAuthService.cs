using katlog_backend.DTOs;

namespace katlog_backend.Services.Interfaces;

public interface IAuthService
{
   Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
   Task<AuthResponseDto> LoginAsync(LoginDto dto);

}