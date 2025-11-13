using HAS.Domain.Common;

namespace HAS.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private Role() { }

    public Role(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Role name is required", nameof(name));
        Name = name.Trim();
        Description = description;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Role name is required", nameof(name));
        Name = name.Trim();
    }
}
