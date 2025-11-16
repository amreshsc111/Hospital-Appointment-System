using HAS.Domain.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HAS.Infrastructure.Identity;

public class RefreshTokenService(ApplicationDbContext context) : IRefreshTokenService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string? createdByIp = null)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedByIp = createdByIp
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress)
    {
        var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (existingToken == null || existingToken.IsRevoked) return false;

        existingToken.IsRevoked = true;
        existingToken.DeletedAt = DateTime.UtcNow;
        existingToken.DeletedById = null;
        await _context.SaveChangesAsync();
        return true;
    }
}