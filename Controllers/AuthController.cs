using katlog_backend.DTOs;
using katlog_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace katlog_backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response);
    }
}