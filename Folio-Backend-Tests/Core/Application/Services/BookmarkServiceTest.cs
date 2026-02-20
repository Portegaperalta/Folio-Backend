using FluentAssertions;
using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.Bookmark;
using Folio.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace Folio_Backend_Tests.Core.Application.Services
{
    public class BookmarkServiceTest
    {
        private readonly BookmarkMapper _bookmarkMapper;
        private readonly IBookmarkRepository _mockBookmarkRepository;
        private readonly ICacheService _mockCacheService;
        private readonly BookmarkService _bookmarkService;

        public BookmarkServiceTest()
        {
            _bookmarkMapper = new BookmarkMapper();
            _mockBookmarkRepository = Substitute.For<IBookmarkRepository>();
            _mockCacheService = Substitute.For<ICacheService>();
            _bookmarkService = new BookmarkService(_mockBookmarkRepository, _bookmarkMapper, _mockCacheService);
        }

        // GetAllUserBookmarksAsync tests
        [Fact]
        public async Task GetAllBookmarksAsync_ReturnsBookmarkList()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var fakeUrl = CreateFakeUrl();
            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);
            var bookmarkList = CreateBookmarkList(boomarkEntity);

            _mockBookmarkRepository.GetAllByUserAndFolderIdAsync(userId, folderId)
                                  .Returns(bookmarkList);

            //Act
            var response = await _bookmarkService.GetAllBookmarksAsync(userId, folderId);

            //Assert
            response.Should().BeAssignableTo<IEnumerable<BookmarkDTO>>();
        }

        [Fact]
        public async Task GetAllBookmarksAsync_CallsGetAllAsyncFromBookmarkRepository()
        {
            //Act
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            await _bookmarkService.GetAllBookmarksAsync(userId, folderId);

            //Assert
            await _mockBookmarkRepository.Received(1).GetAllByUserAndFolderIdAsync(userId, folderId);
        }

        // GetUserBookmarkByIdAsync tests
        [Fact]
        public async Task GetBookmarkByIdAsync_ReturnsNull_WhenBookmarkDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns((Bookmark?)null);

            //Act
            var response = await _bookmarkService.GetBookmarkByIdAsync(userId, folderId, bookmarkId);

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetBookmarkByIdAsync_ReturnsNull_WhenBookmarkDoesNotBelongToUser()
        {
            //Arrange
            Guid unauthorizedUserId = Guid.NewGuid();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();

            //Act 
            var result = await _bookmarkService.GetBookmarkByIdAsync(unauthorizedUserId, folderId, bookmarkId);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBookmarkByIdAsync_ReturnsBookmarkDTO()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns(boomarkEntity);

            //Act
            var result = await _bookmarkService.GetBookmarkByIdAsync(userId, folderId, bookmarkId);

            //Assert
            result.Should().BeOfType<BookmarkDTO>();
        }

        // CountBookmarksByFolderIdAsync tests
        [Fact]
        public async Task 
            CountBookmarksByFolderIdAsync_CallsCountByFolderAsyncFromBookmarkRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockBookmarkRepository.CountByFolderAsync(userId, folderId).Returns(1);

            //Act
            await _bookmarkService.CountBookmarksByFolderIdAsync(userId, folderId);

            //Assert
            await _mockBookmarkRepository.Received(1).CountByFolderAsync(userId, folderId);
        }

        [Fact]
        public async Task CountBookmarksByFolderIdAsync_ReturnsInteger()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockBookmarkRepository.CountByFolderAsync(userId, folderId).Returns(1);

            //Act
            var result = await _bookmarkService.CountBookmarksByFolderIdAsync(userId, folderId);

            //Assert
            result.Should().BeGreaterThanOrEqualTo(0);
            result.Should().Be(1);
        }

        // CreateUserBookmarkAsync tests
        [Fact]
        public async Task 
            CreateBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkCreationDTOIsNull()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var fakeUrl = CreateFakeUrl();

            BookmarkCreationDTO nullBookmarkCreationDTO = null!;

            //Act
            Func<Task> act = async() => await _bookmarkService.CreateBookmarkAsync(userId, folderId, nullBookmarkCreationDTO);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateBookmarkAsync_CallsAddAsyncFromBookmarkRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var folderEntity = CreateFolderEntity("mockFolder", userId);

            var bookmarkCreationDTO = CreateBookmarkCreationDTO("mockBookmark", fakeUrl);

            _mockBookmarkRepository.GetFolderByIdAsync(folderId, userId)
                                  .Returns(folderEntity);

            //Act
            await _bookmarkService.CreateBookmarkAsync(userId, folderId, bookmarkCreationDTO);

            //Assert
            await _mockBookmarkRepository.Received(1).AddAsync(Arg.Any<Bookmark>());
        }

        // UpdateBookmarkAsync tests
        [Fact]
        public async Task 
            UpdateBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkUpdateDTOIsNull()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var fakeUrl = CreateFakeUrl();

            BookmarkUpdateDTO nullBookmarkUpdateDTO = null!;

            //Act
            Func<Task> act = async () => await _bookmarkService.UpdateBookmarkAsync(userId, folderId, nullBookmarkUpdateDTO);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task 
            UpdateBookmarkAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                   .Returns((Bookmark?)null);

            var bookmarkUpdateDTO = CreateBookmarkUpdateDTO(bookmarkId, "mockBookmark", fakeUrl, true);

            //Act
            Func<Task> act = async () => await _bookmarkService.UpdateBookmarkAsync(userId, folderId, bookmarkUpdateDTO);

            //Assert
            await act.Should().ThrowAsync<BookmarkNotFoundException>();
        }

        [Fact]
        public async Task UpdateBookmarkAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns(boomarkEntity);

            var bookmarkUpdateDTO = CreateBookmarkUpdateDTO(bookmarkId, "mockBookmark", fakeUrl, true);

            //Act
            await _bookmarkService.UpdateBookmarkAsync(userId, folderId, bookmarkUpdateDTO);

            //Assert
            await _mockBookmarkRepository.Received(1).UpdateAsync(Arg.Any<Bookmark>());
        }

        // DeleteUserBookmarkAsync tests
        [Fact]
        public async Task 
            DeleteBookmarkAsync_ReturnsFalse_WhenBookmarkDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                   .Returns((Bookmark?)null);

            //Act
            var result = await _bookmarkService.DeleteBookmarkAsync(userId, folderId, bookmarkId);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteBookmarkAsync_ReturnsTrue_WhenBookmarkIsDelete()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns(boomarkEntity);

            //Act 
            var result = await _bookmarkService.DeleteBookmarkAsync(userId, folderId, bookmarkId);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteBookmarkAsync_CallsDeleteAsyncFromBookmarkRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                   .Returns(boomarkEntity);

            //Act
            await _bookmarkService.DeleteBookmarkAsync(userId, folderId, bookmarkId);

            //Assert
            await _mockBookmarkRepository.Received(1).DeleteAsync(Arg.Any<Bookmark>());
        }

        // MarkUserBookmarkAsVisitedAsync tests
        [Fact]
        public async Task MarkBookmarkAsVisitedAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns((Bookmark?)null);

            //Act
            Func<Task> act = async () => await _bookmarkService.MarkBookmarkAsVisitedAsync(userId, folderId, bookmarkId);

            //Assert
            await act.Should().ThrowAsync<BookmarkNotFoundException>();
        }

        [Fact]
        public async Task MarkBookmarkAsVisitedAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var bookmarkId = CreateBookmarkId();
            var fakeUrl = CreateFakeUrl();

            var boomarkEntity = CreateBookmarkEntity("mockBookmark", fakeUrl, folderId, userId);

            _mockBookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId)
                                  .Returns(boomarkEntity);

            //Act
            await _bookmarkService.MarkBookmarkAsVisitedAsync(userId, folderId, bookmarkId);

            //Assert
            await _mockBookmarkRepository.Received(1).UpdateAsync(boomarkEntity);
        }


        // Helper methods
        private Guid CreateUserId() => new();

        private Guid CreateBookmarkId() => new();

        private Guid CreateFolderId() => new();

        private string CreateFakeUrl() => "https://fakeurl.com";

        private Bookmark CreateBookmarkEntity(string name, string url, Guid folderId, Guid userId)
        {
            return new Bookmark(name, url, folderId, userId);
        }

        private Folder CreateFolderEntity(string name, Guid userId)
        {
            return new Folder(name, userId);
        }

        private BookmarkCreationDTO CreateBookmarkCreationDTO(string name, string url)
        {
            return new BookmarkCreationDTO
            {
                Name = name,
                Url = url
            };
        }

        private BookmarkUpdateDTO CreateBookmarkUpdateDTO(Guid bookmarkId, 
            string name, string url, bool isMarkedFavorite = false)
        {
            return new BookmarkUpdateDTO
            {
                Id = bookmarkId,
                Name = name,
                Url = url,
                IsMarkedFavorite = isMarkedFavorite
            };
        }

        private IEnumerable<Bookmark> CreateBookmarkList(Bookmark boomarkEntity)
        {
            return new List<Bookmark> { boomarkEntity };
        }
    }
}
