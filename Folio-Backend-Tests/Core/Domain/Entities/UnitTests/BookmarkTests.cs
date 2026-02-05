using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    [TestClass]
    public class BookmarkTests
    {
        private readonly string MockBookmarkName = "mock bookmark";
        private readonly string MockBookmarkUrl = "https://fakeurl.com";
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly Guid MockFolderId = Guid.NewGuid();
        private Bookmark MockBookmark = null!;

        [TestInitialize]
        public void Setup()
        {
            MockBookmark = new(MockBookmarkName, MockBookmarkUrl, MockFolderId, MockUserId);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void 
            ChangeName_ThrowsEmptyBookmarkNameException_WhenNewNameIsNullOrEmpty(string? newName)
        {
            //Act + Assert
            Assert.Throws<EmptyBookmarkNameException>(() => MockBookmark.ChangeName(newName!));
        }

        [TestMethod]
        public void ChangeName_ShouldUpdateBookmarkName()
        {
            //Arrange
            string newBookmarkName = "newName";

            //Act
            MockBookmark.ChangeName(newBookmarkName);

            //Assert
            Assert.AreEqual(expected: newBookmarkName, actual: MockBookmark.Name);
        }

        [TestMethod]
        public void ChangeUrl_ThrowsArgumentException_WhenNewBookmarkUrlIsNull()
        {
            //Arrange
            string newBookmarkUrl = null!;

            //Act + Assert
            Assert.Throws<ArgumentException>(() => MockBookmark.ChangeUrl(newBookmarkUrl));
        }

        [TestMethod]
        public void ChangeUrl_ShouldUpdateBookmarkUrl()
        {
            //Arrange
            string newBookmarkUrl = "https://newFakeUrl.com";

            //Act
            MockBookmark.ChangeUrl(newBookmarkUrl);

            //Assert
            Assert.AreEqual(expected: newBookmarkUrl, actual: MockBookmark.Url);
        }

        [TestMethod]
        public void MarkFavorite_ShouldSetIsMarkedFavoriteTrue()
        {
            //Act
            MockBookmark.MarkFavorite();

            //Assert
            Assert.IsTrue(MockBookmark.IsMarkedFavorite);
        }

        [TestMethod]
        public void UnmarkFavorite_ShouldSetIsMarkedFavoriteFalse()
        {
            //Act
            MockBookmark.UnmarkFavorite();

            //Assert
            Assert.IsFalse(MockBookmark.IsMarkedFavorite);
        }

        [TestMethod]
        public void Visit_ShouldUpdateLastVisitedTime()
        {
            //Act
            MockBookmark.Visit();

            //Assert
            Assert.IsNotNull(MockBookmark.LastVisitedTime);
        }
    }
}
