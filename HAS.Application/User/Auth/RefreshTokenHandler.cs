using HAS.Application.Common.Interfaces;
using HAS.Domain.Common.Interfaces;

namespace HAS.Application.User.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;

public class RefreshTokenHandler(IRefreshTokenRepository refreshRepo, IUserRepository userRepo, IJwtTokenService jwt, IRefreshTokenService refresh) : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IRefreshTokenRepository _refreshRepo = refreshRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IJwtTokenService _jwt = jwt;
    private readonly IRefreshTokenService _refresh = refresh;

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await _refreshRepo.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existing == null || existing.IsRevoked || existing.Expires < DateTime.UtcNow)
            throw new Exception("Invalid refresh token");


        var user = await _userRepo.GetByIdAsync(existing.UserId, cancellationToken);
        if (user == null) throw new Exception("User not found");


        existing.IsRevoked = true;
        var newRefresh = await _refresh.GenerateRefreshTokenAsync(user.Id);
        await _refreshRepo.AddAsync(newRefresh, cancellationToken);
        await _refreshRepo.SaveChangesAsync(cancellationToken);


        var token = _jwt.GenerateToken(user);
        return new LoginResponse(user.Id, token, newRefresh.Token);
    }
}
