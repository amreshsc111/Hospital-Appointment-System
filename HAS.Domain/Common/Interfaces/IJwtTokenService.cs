using HAS.Domain.Entities;

namespace HAS.Domain.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
