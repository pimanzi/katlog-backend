using Katlog.Api.Models;

namespace Katlog.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user, IList<string> roles);
}