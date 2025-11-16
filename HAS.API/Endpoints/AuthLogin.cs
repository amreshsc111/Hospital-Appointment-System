using HAS.Domain.Common.Interfaces;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HAS.API.Endpoints;

public static class AuthLogin
{
    public static IEndpointRouteBuilder MapAuthLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            [FromBody] LoginRequest request,
            ApplicationDbContext context,
            IPasswordHasher hasher,
            IJwtTokenService jwt,
            IRefreshTokenService refreshTokenService
        ) =>
        { 
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null) return Results.Unauthorized();

            if (!hasher.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            // Generate JWT token
            var jwtToken = jwt.GenerateToken(user);

            // Generate Refresh Token
            var refresh = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            await context.RefreshTokens.AddAsync(refresh);
            await context.SaveChangesAsync();

            return Results.Ok(new LoginResponse
            {
                JwtToken = jwtToken,
                RefreshToken = refresh.Token
            });
        })
        .WithTags("Auth")
        .AllowAnonymous();

        return app;
    }
}

public record LoginRequest(string UserName, string Password);

public class LoginResponse
{
    public string? JwtToken { get; set; }
    public object? RefreshToken { get; set; }
}
