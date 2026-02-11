using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAsync(User userEntity);
        Task DeleteUserAsync(User userEntity);
    }
}
