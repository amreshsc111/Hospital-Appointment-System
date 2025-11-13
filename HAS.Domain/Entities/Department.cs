using HAS.Domain.Common;

namespace HAS.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private Department() { }

    public Department(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Department name is required", nameof(name));
        Name = name.Trim();
        Description = description;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Department name is required", nameof(name));
        Name = name.Trim();
        TouchModified();
    }
}
