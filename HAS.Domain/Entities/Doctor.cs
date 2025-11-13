using HAS.Domain.Common;
using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FullName { get; private set; }
    public string Qualification { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Email? Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }

    private Doctor() { }

    public Doctor(string fullName, string qualification, Guid departmentId, Email? email = null, PhoneNumber? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Doctor name is required", nameof(fullName));
        FullName = fullName.Trim();
        Qualification = qualification?.Trim() ?? string.Empty;
        DepartmentId = departmentId;
        Email = email;
        Phone = phone;
    }

    public void UpdateProfile(string? fullName = null, string? qualification = null, Email? email = null, PhoneNumber? phone = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName)) FullName = fullName.Trim();
        if (!string.IsNullOrWhiteSpace(qualification)) Qualification = qualification.Trim();
        if (email != null) Email = email;
        if (phone != null) Phone = phone;
        TouchModified();
    }
}