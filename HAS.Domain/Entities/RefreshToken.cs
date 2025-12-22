using HAS.Domain.Common;

namespace HAS.Domain.Entities;

public class RefreshToken : BaseEntity
{
    // Foreign key to User entity (no navigation property defined)
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public string? CreatedByIp { get; set; }
}
