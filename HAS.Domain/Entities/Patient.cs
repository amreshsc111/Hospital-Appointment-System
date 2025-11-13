using HAS.Domain.Common;
using HAS.Domain.Enums;
using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class Patient : BaseEntity
{
    public Email Email { get; private set; }
    public string FullName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public PhoneNumber? Phone { get; private set; }

    private Patient() { }

    public Patient(string fullName, Email email, DateTime dateOfBirth, Gender gender, PhoneNumber? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Full name is required", nameof(fullName));
        FullName = fullName.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Phone = phone;
    }

    public void UpdateContact(Email email, PhoneNumber? phone)
    {
        Email = email ?? Email;
        Phone = phone ?? Phone;
        TouchModified();
    }
}
