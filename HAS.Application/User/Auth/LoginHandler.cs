using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Common.Interfaces;

namespace HAS.Application.User.Auth;

public record LoginCommand(string UserName, string Password) : IRequest<LoginResponse>;
public record LoginResponse(Guid UserId, string Token, string RefreshToken);

public class LoginHandler(IUserRepository users, IPasswordHasher hasher, IJwtTokenService jwt,
    IRefreshTokenRepository refreshRepo, IRefreshTokenService refresh) : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users = users;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IJwtTokenService _jwt = jwt;
    private readonly IRefreshTokenRepository _refreshRepo = refreshRepo;
    private readonly IRefreshTokenService _refresh = refresh;

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByUserNameAsync(request.UserName, cancellationToken);
        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        var token = _jwt.GenerateToken(user);
        var refresh = await _refresh.GenerateRefreshTokenAsync(user.Id, null);
        await _refreshRepo.AddAsync(refresh, cancellationToken);
        await _refreshRepo.SaveChangesAsync(cancellationToken);

        return new LoginResponse(user.Id, token, refresh.Token);
    }
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
