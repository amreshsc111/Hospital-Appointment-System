using HAS.Domain.Entities;

namespace HAS.Domain.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string? createdByIp = null);
    Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress);
}