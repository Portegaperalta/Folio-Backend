using FluentAssertions;
using Folio.Core.Application.DTOs.User;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.User;
using Folio.Core.Interfaces;
using NSubstitute;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Folio_Backend_Tests.Core.Application.Services
{
    public class UserServiceTests
    {

        private readonly IUserRepository _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = Substitute.For<IUserRepository>();
            _userService = new UserService(_mockUserRepository);
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();

            _mockUserRepository.GetUserByIdAsync(userId)
                               .Returns((User?)null);

            var updateUserDTO = createUserUpdateDTO(userId, "newUserName", "newFake@mock.com", null);

            //Act 
            Func<Task> act = async () => await _userService.UpdateUserAsync(userId, updateUserDTO);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsArgumentException_WhenUserIdsDoNotMatch()
        {
            //Arrange
            var userId = CreateUserId();
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword("!Fakepassword123");

            var userEntity = CreateUserEntity(userId, userName, userEmail, userPassword);

            _mockUserRepository.GetUserByIdAsync(userId)
                               .Returns(userEntity);

            var mismatchUpdateDTO = new UserUpdateDTO
            {
                UserId = Guid.NewGuid(),
                Name = "fakeUsername"
            };

            //Act 
            Func<Task> act = async () => await _userService.UpdateUserAsync(userId, mismatchUpdateDTO);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateUserAsync_CallsUpdateUserAsyncFromUserRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword("!Fakepassword123");

            var userEntity = CreateUserEntity(userId, userName, userEmail, userPassword);

            _mockUserRepository.GetUserByIdAsync(userId)
                               .Returns(userEntity);

            var updateUserDTO = createUserUpdateDTO(userId, "newUserName", "newFake@mock.com", null);

            //Act
            await _userService.UpdateUserAsync(userId, updateUserDTO);

            //Assert
            await _mockUserRepository.Received(1).UpdateUserAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task DeleteUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var userEmail = CreateUserEmail();

            _mockUserRepository.GetUserByIdAsync(userId)
                               .Returns((User?)null);

            //Act 
            Func<Task> act = async () => await _userService.DeleteUserAsync(userId);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task DeleteUserAsync_CallsDeleteUserAsyncFromUserRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword("!Fakepassword123");

            var userEntity = CreateUserEntity(userId, userName, userEmail, userPassword);

            _mockUserRepository.GetUserByIdAsync(userId)
                               .Returns(userEntity);

            //Act
            await _userService.DeleteUserAsync(userId);

            //Assert
            await _mockUserRepository.Received(1).DeleteUserAsync(Arg.Any<User>());
        }

        private Guid CreateUserId() => new();

        private string CreateUserName() => "mockUserName";

        private string CreateUserEmail() => "fake@example.com";

        private string CreateUserPassword(string password)
        {
            var inputBytes = Encoding.UTF8.GetBytes(password);

            var inputHash = SHA256.HashData(inputBytes);

            return Convert.ToHexString(inputHash);
        }

        private User CreateUserEntity(Guid userId, string userName, string userEmail,
            string passwordHash, string phonewNumber = null!)
        {
            return new User(userName, userEmail, passwordHash, phonewNumber);
        }

        private UserUpdateDTO createUserUpdateDTO(Guid userId, string? newName, string? newEmail,
            string? newPhoneNumber)
        {
            return new UserUpdateDTO
            {
              UserId = userId,
              Name = newName,
              Email = newEmail,
              PhoneNumber = newPhoneNumber
            };
        }
    }
}
