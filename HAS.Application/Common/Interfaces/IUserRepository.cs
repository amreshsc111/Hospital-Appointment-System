using UserEntity = HAS.Domain.Entities.User;

namespace HAS.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<UserEntity>> GetAllAsync(CancellationToken cancellation);
    Task AddAsync(UserEntity user, CancellationToken cancellation);
    Task<bool> AnyByUserNameOrEmailAsync(string userName, string email, CancellationToken cancellationToken);
}
