using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Roles).ToListAsync(cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public Task<bool> AnyByUserNameOrEmailAsync(string userName, string email, CancellationToken cancellationToken) =>
        _dbContext.Users.AnyAsync(u => u.UserName == userName || u.Email.Value == email, cancellationToken);
}
