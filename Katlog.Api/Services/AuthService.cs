using Katlog.Api.DTOs;
using Katlog.Api.Exceptions;
using Katlog.Api.Models;
using Katlog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Katlog.Api.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager
            .CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First().Description;
            throw new BadRequestException(error);
        }
        
        await _userManager.AddToRoleAsync(user, "Admin");

    
        var roles = await _userManager.GetRolesAsync(user);
        
        var token = _tokenService.GenerateToken(user, roles);
        
        return new AuthResponseDto(
            token,
            user.Email,
            roles.ToList()
        );
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var existingUser = await _userManager
            .FindByEmailAsync(dto.Email);
        
        if (existingUser is null)
        {
            throw new BadRequestException("Invalid email or password");
        }
        
        var isPasswordValid = await _userManager
            .CheckPasswordAsync(existingUser, dto.Password);

        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(existingUser);
        var token = _tokenService.GenerateToken(existingUser, roles);

        return new AuthResponseDto(
            token,
            existingUser.Email!,
            roles.ToList()
        );
    }
}