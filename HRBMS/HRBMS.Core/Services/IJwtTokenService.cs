using HRBMS.Core.Entities;

namespace HRBMS.Core.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user, IEnumerable<string> roleNames);
}