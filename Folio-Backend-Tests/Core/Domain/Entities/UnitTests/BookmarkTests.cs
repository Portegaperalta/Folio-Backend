using FluentAssertions;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.Bookmark;
using Xunit;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    public class BookmarkTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void 
            ChangeName_ThrowsEmptyBookmarkNameException_WhenNewNameIsNullOrEmpty(string? newName)
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);

            //Act
            Action act = () => bookmarkEntity.ChangeName(newName!);

            //Assert
            act.Should().Throw<EmptyBookmarkNameException>();
        }

        [Fact]
        public void ChangeName_ShouldUpdateBookmarkName()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);
            
            string newBookmarkName = "newName";

            //Act
            bookmarkEntity.ChangeName(newBookmarkName);

            //Assert
            bookmarkEntity.Name.Should().BeSameAs(newBookmarkName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeUrl_ThrowsEmptyBookmarkUrlException_WhenNewUrlIsNullOrEmpty(string? newUrl)
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);

            //Act
            Action act = () => bookmarkEntity.ChangeUrl(newUrl!);

            //Assert
            act.Should().Throw<EmptyBookmarkUrlException>();
        }

        [Fact]
        public void ChangeUrl_ShouldUpdateBookmarkUrl()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);
           
            string newBookmarkUrl = "https://newFakeUrl.com";

            //Act
            bookmarkEntity.ChangeUrl(newBookmarkUrl);

            //Assert
            bookmarkEntity.Url.Should().BeSameAs(newBookmarkUrl);
        }

        [Fact]
        public void MarkFavorite_ShouldSetIsMarkedFavoriteTrue()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);

            //Act
            bookmarkEntity.MarkFavorite();

            //Assert
            bookmarkEntity.IsMarkedFavorite.Should().BeTrue();
        }

        [Fact]
        public void UnmarkFavorite_ShouldSetIsMarkedFavoriteFalse()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);

            //Act
            bookmarkEntity.UnmarkFavorite();

            //Assert
            bookmarkEntity.IsMarkedFavorite.Should().BeFalse();
        }

        [Fact]
        public void Visit_ShouldUpdateLastVisitedTime()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkName = CreateBookmarkName();
            var bookmarkUrl = CreateBookmarkUrl();
            var bookmarkEntity = CreateBookmarkEntity(bookmarkName, bookmarkUrl, folderId, userId);

            //Act
            bookmarkEntity.Visit();

            //Assert
            bookmarkEntity.LastVisitedTime.Should().NotBeNull();
        }

        //Helper methods
        private string CreateBookmarkName() => "mockBookmark";
        private string CreateBookmarkUrl() => "https://fakeurl.com";
        private Guid CreateUserId() => new();
        private Guid CreateFolderId() => new();

        private Bookmark CreateBookmarkEntity(string name, string url, Guid folderId, Guid userId)
        {
            return new Bookmark(name, url, folderId, userId);
        }
    }
}
