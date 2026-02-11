using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.User;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    [TestClass]
    public class UserTests
    {
        private readonly string MockUserName = "fakeName";
        private readonly string MockUserEmail = "fake@example.com";
        private readonly string MockPasswordHash = "fakepasswordhash";
        private readonly string MockPhoneNumber = "+18880009999";
        private User MockUser = null!;

        [TestInitialize]
        public void Setup()
        {
            MockUser = new(MockUserName, MockUserEmail, MockPasswordHash, MockPhoneNumber);
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void 
            ChangeName_ThrowsEmptyUsernameException_WhenNewNameIsNullOrWhiteSpace(string? newName)
        {
            //Arrange
            string newUserName = newName!;

            //Act + Assert
            Assert.Throws<EmptyUsernameException>(() => MockUser.ChangeName(newUserName));
        }

        [TestMethod]
        public void ChangeName_UpdatesUserName()
        {
            //Arrange 
            string newUserName = "newFakeName";

            //Act
            MockUser.ChangeName(newUserName);

            //Assert
            Assert.AreEqual(expected: newUserName, actual: MockUser.Name);
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void 
            ChangeEmail_ThrowsEmptyUserEmailException_WhenNewEmaileIsNullOrWhiteSpace(string? newEmail)
        {
            //Arrange
            string newUserEmail = newEmail!;

            //Act + Assert
            Assert.Throws<EmptyUserEmailException>(() => MockUser.ChangeEmail(newUserEmail));
        }

        [TestMethod]
        public void ChangeEmail_UpdatesUserEmail()
        {
            //Arrange 
            string newUserEmail = "newFakeEmail@example.com";

            //Act
            MockUser.ChangeEmail(newUserEmail);

            //Assert
            Assert.AreEqual(expected: newUserEmail, actual: MockUser.Email);
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void 
            ChangePassword_ThrowsEmptyUserPasswordHashException_WhenNewPasswordHashIsNullOrWhiteSpace(string? newPasswordHash)
        {
            //Arrange
            string newUserPasswordHash = newPasswordHash!;

            //Act + Assert
            Assert.Throws<EmptyUserPasswordHashException>(() => MockUser.ChangePassword(newUserPasswordHash));
        }

        [TestMethod]
        public void ChangePassword_ShouldUpdatesPasswordHash()
        {
            //Arrange
            string newPasswordHash = "e3d7cf509e5ffc03d30a20cdf3b513119e8f647c1afe49caadc5766fd13f3d18";

            //Act
            MockUser.ChangePassword(newPasswordHash);

            //Assert
            Assert.AreEqual(expected: newPasswordHash, actual: MockUser.PasswordHash);
        }

        [TestMethod]
        [DataRow("+18887778888")]
        [DataRow(null)]
        public void SetPhoneNumber_UpdatesPhoneNumber(string? PhoneNumber)
        {
            //Arrange 
            string newPhoneNumber = PhoneNumber!;

            //Act 
            MockUser.SetPhoneNumber(newPhoneNumber);

            //Assert
            Assert.AreEqual(expected: newPhoneNumber, actual: MockUser.PhoneNumber);
        }

        [TestMethod]
        public void Delete_ShouldSetIsDeletedTrue()
        {
            //Act
            MockUser.Delete();

            //Assert
            Assert.IsTrue(MockUser.IsDeleted);
        }
    }
}
