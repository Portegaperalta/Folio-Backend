using Folio.Core.Application.DTOs.User;

namespace Folio.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDetailsDTO> GetUserProfileDetails(Guid userId);
        Task UpdateUserAsync(Guid userId, UserUpdateDTO userUpdateDTO);
        Task DeleteUserAsync(Guid userId);
    }
}
