using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
}
