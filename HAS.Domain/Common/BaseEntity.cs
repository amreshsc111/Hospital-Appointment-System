using HAS.Domain.Common.Interfaces;

namespace HAS.Domain.Common;

public abstract class BaseEntity : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }

    // Auditing
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedById { get; set; }

    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}
