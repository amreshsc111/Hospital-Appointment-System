using HAS.Domain.Common.Interfaces;
using HAS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HAS.Infrastructure.Identity;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(User user)
    {
        var secret = _configuration["Jwt:Secret"] ?? throw new Exception("JWT Secret not configured");
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        // Add role claims for authorization
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(AuthorizationPolicies.RoleClaimType, role.Name));
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}