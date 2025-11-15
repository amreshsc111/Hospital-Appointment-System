namespace HAS.Domain.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}
