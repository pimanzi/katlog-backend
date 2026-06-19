using katlog_backend.Models;

namespace katlog_backend.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user, IList<string> roles);
}