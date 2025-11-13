namespace HAS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.Now;
    public Guid CreatedById { get; protected set; }
    public DateTime? ModifiedAt { get; protected set; }
    public Guid? ModifiedById { get; protected set; }

    protected void TouchModified() => ModifiedAt = DateTime.Now;
}
