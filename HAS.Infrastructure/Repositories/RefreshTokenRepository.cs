using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _db;
    public RefreshTokenRepository(ApplicationDbContext db) => _db = db;

    public Task AddAsync(RefreshToken token, CancellationToken ct = default) =>
        _db.RefreshTokens.AddAsync(token, ct).AsTask();

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
