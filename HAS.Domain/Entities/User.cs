using HAS.Domain.ValueObjects;

namespace HAS.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Email Email { get; private set; }
    public string UserName { get; private set; }

    // Password handling is done by infrastructure (hashing). Domain stores hash and salt.
    public string PasswordHash { get; private set; } = string.Empty;
    public string? FullName { get; private set; }
    public bool IsActive { get; private set; } = true;

    public List<Role> Roles { get; private set; } = new();

    private User() { }

    public User(string userName, Email email, string passwordHash, string? fullName = null)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username is required", nameof(userName));
        UserName = userName.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FullName = fullName;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required", nameof(passwordHash));
        PasswordHash = passwordHash;
    }

    public void AddRole(Role role)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (!Roles.Any(r => r.Id == role.Id)) Roles.Add(role);
    }

    public void RemoveRole(Guid roleId)
    {
        var existing = Roles.FirstOrDefault(r => r.Id == roleId);
        if (existing != null) Roles.Remove(existing);
    }

    public bool HasRole(string roleName) => Roles.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));

    public void Deactivate() { IsActive = false; }
    public void Activate() { IsActive = true; }
}