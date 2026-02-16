using FluentAssertions;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.User;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    public class UserTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void 
            ChangeName_ThrowsEmptyUsernameException_WhenNewNameIsNullOrWhiteSpace(string? newName)
        {
            //Arrange
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            string newUserName = newName!;

            Action act = () => user.ChangeName(newUserName);

            //Assert
            act.Should().Throw<EmptyUsernameException>();
        }

        [Fact]
        public void ChangeName_UpdatesUserName()
        {
            //Arrange 
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            string newUserName = "newFakeName";

            user.ChangeName(newUserName);

            //Assert
            user.Name.Should().BeSameAs(newUserName);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void 
            ChangeEmail_ThrowsEmptyUserEmailException_WhenNewEmaileIsNullOrWhiteSpace(string? newEmail)
        {
            //Arrange
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            string newUserEmail = newEmail!;

            Action act = () => user.ChangeEmail(newUserEmail);

            //Assert
            act.Should().Throw<EmptyUserEmailException>();
        }

        [Fact]
        public void ChangeEmail_UpdatesUserEmail()
        {
            //Arrange 
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            string newUserEmail = "newFakeEmail@example.com";

            user.ChangeEmail(newUserEmail);

            //Assert
            user.Email.Should().BeSameAs(newUserEmail);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void 
            ChangePassword_ThrowsEmptyUserPasswordHashException_WhenNewPasswordHashIsNullOrWhiteSpace(string? newPasswordHash)
        {
            //Arrange
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            string newUserPasswordHash = newPasswordHash!;

            Action act = () => user.ChangePassword(newUserPasswordHash);

            //Assert
            act.Should().Throw<EmptyUserPasswordHashException>();
        }

        [Fact]
        public void ChangePassword_ShouldUpdatesPasswordHash()
        {
            //Arrange
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            var newPasswordHash = CreateUserPasswordHash("#Newfakepassword123");

            user.ChangePassword(newPasswordHash);

            //Assert
            user.PasswordHash.Should().BeSameAs(newPasswordHash);
        }

        [Theory]
        [InlineData("+18887778888")]
        [InlineData(null)]
        public void SetPhoneNumber_UpdatesPhoneNumber(string? PhoneNumber)
        {
            //Arrange 
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act 
            string newPhoneNumber = PhoneNumber!;

            user.SetPhoneNumber(newPhoneNumber);

            //Assert
            user.PhoneNumber.Should().BeSameAs(newPhoneNumber);
        }

        [Fact]
        public void Delete_ShouldSetIsDeletedTrue()
        {
            //Arrange
            var userName = CreateUserName();
            var userEmail = CreateUserEmail();
            var userPassword = CreateUserPassword();
            var userPhoneNumber = CreateUserPhoneNumber();
            var userPasswordHash = CreateUserPasswordHash(userPassword);

            var user = CreateUser(userName, userEmail, userPasswordHash, userPhoneNumber);

            //Act
            user.Delete();

            //Assert
            user.IsDeleted.Should().BeTrue();
        }

        //Helper methods
        private string CreateUserName() => "fakeName";
        private string CreateUserEmail() => "fake@example.com";
        private string CreateUserPhoneNumber() => "+18880009999";
        private string CreateUserPassword() => "#Fakepassword123";

        private string CreateUserPasswordHash(string password)
        {
            var inputBytes = Encoding.UTF8.GetBytes(password);

            var inputHash = SHA256.HashData(inputBytes);

            return Convert.ToHexString(inputHash);
        }

        private User CreateUser(string name, string email, string passwordHash, string? phoneNumber)
        {
            return new User(name, email, passwordHash, phoneNumber);
        }
    }
}
