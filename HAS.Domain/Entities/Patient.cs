using HAS.Domain.Common;
using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class Patient : BaseEntity
{
    public string FullName { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public Email Email { get; set; } = default!;
    public PhoneNumber PhoneNumber { get; set; } = default!;
}
