using FluentAssertions;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.Bookmark;
using Folio.Core.Domain.Exceptions.Folder;
using Xunit;

namespace Folio_Backend_Tests.Core.Domain.Entities.UnitTests
{
    public class FolderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void
            ChangeName_ThrowsEmptyFolderNameException_WhenNewNameIsNullOrEmpty(string? newName)
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            //Act
            Action act = () => folderEntity.ChangeName(newName!);

            //Assert
            act.Should().Throw<EmptyFolderNameException>();
        }

        [Fact]
        public void ChangeName_ShouldUpdateName()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            //Act
            string newFolderName = "new name";
            folderEntity.ChangeName(newFolderName);

            //Assert
            folderEntity.Name.Should().BeSameAs(newFolderName);
        }

        [Fact]
        public void MarkFavorite_ShouldSetIsMarkedFavoriteTrue()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            //Act
            folderEntity.MarkFavorite();

            //Assert
            folderEntity.IsMarkedFavorite.Should().BeTrue();
        }

        [Fact]
        public void UnmarkFavorite_ShouldSetIsMarkedFavoriteFalse()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            //Act
            folderEntity.UnmarkFavorite();

            //Assert
            folderEntity.IsMarkedFavorite.Should().BeFalse();
        }

        [Fact]
        public void Visit_ShouldUpdateLastVisitedTime()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            //Act
            folderEntity.Visit();

            //Assert
            folderEntity.LastVisitedTime.Should().NotBeNull();
        }

        [Fact]
        public void AddBookmark_ThrowsArgumentNullException_WhenNewBookmarkIsNull()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            Bookmark nullBookmarkEntity = null!;

            //Act
            Action act = () => folderEntity.AddBookmark(nullBookmarkEntity);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddBookmark_ThrowsArgumentException_WhenBookmarkFolderIsNotEqualToFolderId()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);

            Guid otherFolderId = Guid.NewGuid();
            Bookmark unauthorizedBookmark = new("otherFakeBookmark", "https://otherFakeUrl", otherFolderId, userId);

            //Act
            Action act = () => folderEntity.AddBookmark(unauthorizedBookmark);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddBookmark_ShouldAddBookmarkToList()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);
            var bookmarkEntity = CreateBookmarkEntity(folderEntity.Id, userId);

            //Act
            folderEntity.AddBookmark(bookmarkEntity);

            //Assert
            folderEntity.Bookmarks.Should().AllBeEquivalentTo(bookmarkEntity);
        }

        [Fact]
        public void RemoveBookmark_ShouldRemoveBookmarkFromListIfBookmarkExistsInList()
        {
            //Arrange
            var userId = CreateUserId();
            var folderName = CreateFolderName();
            var folderEntity = CreateFolderEntity(folderName, userId);
            var bookmarkEntity = CreateBookmarkEntity(folderEntity.Id, userId);

            folderEntity.AddBookmark(bookmarkEntity);

            //Act
            folderEntity.RemoveBookmark(bookmarkEntity.Id);

            //Assert
            folderEntity.Bookmarks.Should().NotContain(bookmarkEntity);
        }

        //Helper methods
        private Guid CreateUserId() => new();
        private string CreateFolderName() => "mock folder";
        
        private Folder CreateFolderEntity(string name, Guid userId)
        {
            return new Folder(name, userId);
        }

        private Bookmark CreateBookmarkEntity(Guid folderId, Guid userId)
        {
            return new Bookmark("mockBookmark", "https://fakeurl.com", folderId, userId);
        }
    }
}
