using HAS.Domain.Common;
using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public Email Email { get; set; } = default!;
    public ICollection<Role> Roles { get; set; } = [];
}