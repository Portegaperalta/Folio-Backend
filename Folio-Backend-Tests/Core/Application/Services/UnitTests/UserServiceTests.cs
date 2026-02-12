using Folio.Core.Application.DTOs.User;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.User;
using Folio.Core.Interfaces;
using NSubstitute;

namespace Folio_Backend_Tests.Core.Application.Services.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly string MockUsername = "fakename";
        private readonly string MockUserEmail = "fake@example.com";
        private readonly string MockUserPhone = "+18880009999";

        private User MockUser = null!;
        private UserUpdateDTO MockUserUpdateDTO = null!;
        private IUserRepository MockUserRepository = null!;
        private UserService userService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockUser = new User(MockUsername, MockUserEmail, "dada", MockUserPhone);

            MockUserUpdateDTO = new UserUpdateDTO
            {
                UserId = MockUserId,
                Name = "newNameMock",
                Email = "fake@mock.com",
            };

            MockUserRepository = Substitute.For<IUserRepository>();
            userService = new UserService(MockUserRepository);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            MockUserRepository.GetUserByIdAsync(MockUserId)
                              .Returns((User?)null);

            //Act + Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() =>
            userService.UpdateUserAsync(MockUserId, MockUserUpdateDTO));
        }

        [TestMethod]
        public async Task UpdateUserAsync_ThrowsArgumentException_WhenUserIdsDoNotMatch()
        {
            //Arrange
            MockUserRepository.GetUserByIdAsync(MockUserId)
                              .Returns(MockUser);

            var mismatchDTO = new UserUpdateDTO
            {
                UserId = Guid.NewGuid(),
                Name = "fakeUsername"
            };

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
            userService.UpdateUserAsync(MockUserId, mismatchDTO));
        }

        [TestMethod]
        public async Task UpdateUserAsync_CallsUpdateUserAsyncFromUserRepository()
        {
            //Arrange
            MockUserRepository.GetUserByIdAsync(MockUserId)
                              .Returns(MockUser);

            //Act
            await userService.UpdateUserAsync(MockUserId, MockUserUpdateDTO);

            //Assert
            await MockUserRepository.Received(1).UpdateUserAsync(Arg.Any<User>());
        }

        [TestMethod]
        public async Task DeleteUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            MockUserRepository.GetUserByIdAsync(MockUserId)
                              .Returns((User?)null);

            //Act + Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() =>
            userService.DeleteUserAsync(MockUserId));
        }

        [TestMethod]
        public async Task DeleteUserAsync_CallsDeleteUserAsyncFromUserRepository()
        {
            //Arrange
            MockUserRepository.GetUserByIdAsync(MockUserId)
                              .Returns(MockUser);

            //Act
            await userService.DeleteUserAsync(MockUserId);

            //Assert
            await MockUserRepository.Received(1).DeleteUserAsync(Arg.Any<User>());
        }
    }
}
