using HAS.Domain.Common;

namespace HAS.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    public ICollection<Doctor> Doctors { get; set; } = [];
}
