using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken);
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken);
}
