using HAS.Domain.Common;
using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public Email Email { get; set; } = default!;
    public PhoneNumber PhoneNumber { get; set; } = default!;
}