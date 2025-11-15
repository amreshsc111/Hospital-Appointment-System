namespace HAS.Domain.Common.Interfaces;

public interface IAuditable
{
    Guid? CreatedById { get; set; }
    Guid? ModifiedById { get; set; }
    Guid? DeletedById { get; set; }
}


public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
