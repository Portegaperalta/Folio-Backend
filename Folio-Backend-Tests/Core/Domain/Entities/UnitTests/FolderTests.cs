using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    [TestClass]
    public class FolderTests
    {
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly string MockFolderName = "mock folder";
        private Bookmark MockBookmark = null!;
        private Folder MockFolder = null!;

        [TestInitialize]
        public void Setup()
        {
            MockFolder = new Folder(MockFolderName, MockUserId);
            MockBookmark = new("mockBookmark", "https://fakeurl.com", MockFolder.Id, MockUserId);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void ChangeName_ThrowsEmptyFolderNameException_WhenNewNameIsNullOrWhiteSpace(string? newName)
        {
            //Act + Assert
            Assert.Throws<EmptyFolderNameException>(() => MockFolder.ChangeName(newName!));
        }

        [TestMethod]
        public void ChangeName_ShouldUpdateName()
        {
            //Act
            string newFolderName = "new name";
            MockFolder.ChangeName(newFolderName);

            //Assert
            Assert.AreEqual(expected: newFolderName, actual: MockFolder.Name);
        }

        [TestMethod]
        public void MarkFavorite_ShouldSetIsMarkedFavoriteTrue()
        {
            //Act
            MockFolder.MarkFavorite();

            //Assert
            Assert.IsTrue(MockFolder.IsMarkedFavorite);
        }

        [TestMethod]
        public void UnmarkFavorite_ShouldSetIsMarkedFavoriteFalse()
        {
            //Act
            MockFolder.UnmarkFavorite();

            //Assert
            Assert.IsFalse(MockFolder.IsMarkedFavorite);
        }

        [TestMethod]
        public void Visit_ShouldUpdateLastVisitedTime()
        {
            //Act
            MockFolder.Visit();

            //Assert
            Assert.IsNotNull(MockFolder.LastVisitedTime);
        }

        [TestMethod]
        public void AddBookmark_ThrowsArgumentNullException_WhenNewBookmarkIsNull()
        {
            //Arrange
            Bookmark nullBookmarkEntity = null!;

            //Act + Assert
            Assert.Throws<ArgumentNullException>(() => MockFolder.AddBookmark(nullBookmarkEntity));
        }

        [TestMethod]
        public void AddBookmark_ThrowsArgumentException_WhenBookmarkFolderIsNotEqualToFolderId()
        {
            //Arrange
            Guid otherFolderId = Guid.NewGuid();
            Bookmark unauthorizedBookmark = new("otherFakeBookmark", "https://otherFakeUrl", otherFolderId, MockUserId);

            //Act + Assert
            Assert.Throws<ArgumentException>(() => MockFolder.AddBookmark(unauthorizedBookmark));
        }

        [TestMethod]
        public void AddBookmark_ShouldAddBookmarkToList()
        {
            //Act
            MockFolder.AddBookmark(MockBookmark);

            //Assert
            CollectionAssert.Contains(MockFolder.Bookmarks.ToList(), MockBookmark);
        }

        [TestMethod]
        public void RemoveBookmark_ShouldRemoveBookmarkFromListIfBookmarkExistsInList()
        {
            //Arrange
            MockFolder.AddBookmark(MockBookmark);

            //Act
            MockFolder.RemoveBookmark(MockBookmark.Id);

            //Assert
            CollectionAssert.DoesNotContain(MockFolder.Bookmarks.ToList(), MockBookmark);
        }
    }
}
