using HAS.Domain.Common;

namespace HAS.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
