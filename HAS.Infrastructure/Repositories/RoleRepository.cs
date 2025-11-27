using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext dbContext) : IRoleRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken) =>
        _dbContext.Roles.Where(r => names.Contains(r.Name)).ToListAsync(cancellationToken);

    public Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
}
