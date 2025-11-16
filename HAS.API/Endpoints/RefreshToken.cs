using HAS.Domain.Common.Interfaces;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HAS.API.Endpoints;

public static class RefreshToken
{
    public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh-token", async (
            [FromBody] RefreshTokenRequest request,
            ApplicationDbContext context,
            IJwtTokenService jwt,
            IRefreshTokenService refreshTokenService
        ) =>
        {
            var existing = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken);
            if (existing == null || existing.IsRevoked || existing.Expires < DateTime.UtcNow)
                return Results.Unauthorized();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == existing.UserId);
            if (user == null) return Results.Unauthorized();

            // Revoke old refresh token
            existing.IsRevoked = true;

            // Create new refresh token
            var newRefresh = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            await context.RefreshTokens.AddAsync(newRefresh);

            // Issue new JWT
            var newJwt = jwt.GenerateToken(user);

            await context.SaveChangesAsync();

            return Results.Ok(new
            {
                token = newJwt,
                refreshToken = newRefresh.Token
            });
        })
        .WithTags("Auth")
        .AllowAnonymous();

        return app;
    }
}

public record RefreshTokenRequest(string RefreshToken);