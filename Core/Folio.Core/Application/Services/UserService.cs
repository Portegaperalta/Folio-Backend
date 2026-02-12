using Folio.Core.Application.DTOs.User;
using Folio.Core.Domain.Exceptions.User;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task UpdateUserAsync(Guid userId, UserUpdateDTO userUpdateDTO)
        {
            var userEntity = await _userRepository.GetUserByIdAsync(userId);

            if (userEntity is null)
                throw new UserNotFoundException(userId);

            if (userId != userUpdateDTO.UserId)
                throw new ArgumentException("User id's must match");

            if (userUpdateDTO.Name is not null)
                userEntity.ChangeName(userUpdateDTO.Name);

            if (userUpdateDTO.Email is not null)
                userEntity.ChangeEmail(userUpdateDTO.Email);

            if (userUpdateDTO.PhoneNumber is not null)
                userEntity.SetPhoneNumber(userUpdateDTO.PhoneNumber);

            await _userRepository.UpdateUserAsync(userEntity);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var userEntity = await _userRepository.GetUserByIdAsync(userId);

            if (userEntity is null)
                throw new UserNotFoundException(userId);

            await _userRepository.DeleteUserAsync(userEntity);
        }
    }
}