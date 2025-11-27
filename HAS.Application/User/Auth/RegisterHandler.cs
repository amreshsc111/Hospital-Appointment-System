using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.ValueObjects;
using UserEntity = HAS.Domain.Entities.User;

namespace HAS.Application.User.Auth;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string? FullName,
    string[]? Roles
) : IRequest<RegisterResult>;
public record RegisterResult(Guid UserId, string Token, string RefreshToken);

public class RegisterHandler(IUserRepository userRepo, IRoleRepository roleRepo, IRefreshTokenRepository refreshRepo, IPasswordHasher hasher, IJwtTokenService jwt, IRefreshTokenService refreshService) : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IRoleRepository _roleRepo = roleRepo;
    private readonly IRefreshTokenRepository _refreshRepo = refreshRepo;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IJwtTokenService _jwt = jwt;
    private readonly IRefreshTokenService _refreshService = refreshService;

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepo.AnyByUserNameOrEmailAsync(request.UserName, request.Email, cancellationToken))
            throw new Exception("User with same username or email already exists");

        var user = new UserEntity
        {
            UserName = request.UserName,
            PasswordHash = _hasher.HashPassword(request.Password),
            FullName = request.FullName ?? request.UserName,
            Email = new Email(request.Email)
        };

        // attach roles
        if (request.Roles != null && request.Roles.Any())
        {
            var roles = await _roleRepo.GetRolesByNamesAsync(request.Roles, cancellationToken);
            foreach (var r in roles) user.Roles.Add(r);
        }
        else
        {
            var userRole = await _roleRepo.GetRoleByNameAsync("User", cancellationToken) ?? new Role { Name = "User" };
            user.Roles.Add(userRole);
        }

        await _userRepo.AddAsync(user, cancellationToken);
        var refresh = await _refreshService.GenerateRefreshTokenAsync(user.Id);
        await _refreshRepo.AddAsync(refresh, cancellationToken);
        await _userRepo.SaveChangesAsync(cancellationToken);

        var token = _jwt.GenerateToken(user);

        return new RegisterResult(user.Id, token, refresh.Token);
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
        .NotEmpty().WithMessage("Username is required")
        .MinimumLength(3).WithMessage("Username must be at least 3 characters");

        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.FullName)
        .MaximumLength(100).WithMessage("Full name must be less than 100 characters")
        .When(x => x.FullName != null);
    }
}
