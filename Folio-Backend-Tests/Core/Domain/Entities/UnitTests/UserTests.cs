using Folio.Core.Domain.Entities;

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
        public void ChangeName_ThrowsArgumentException_WhenNewNameIsNullOrWhiteSpace(string? newName)
        {
            //Arrange
            string newUserName = newName!;

            //Act + Assert
            Assert.Throws<ArgumentException>(() => MockUser.ChangeName(newUserName));
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
        public void ChangeEmail_ThrowsArgumentException_WhenNewEmaileIsNullOrWhiteSpace(string? newEmail)
        {
            //Arrange
            string newUserEmail = newEmail!;

            //Act + Assert
            Assert.Throws<ArgumentException>(() => MockUser.ChangeEmail(newUserEmail));
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
