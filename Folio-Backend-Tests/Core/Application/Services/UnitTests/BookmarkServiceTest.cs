using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Interfaces;
using NSubstitute;

namespace Folio_Backend_Tests.Core.Application.Services.UnitTests
{
    [TestClass]
    public class BookmarkServiceTest
    {
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly Guid MockBookmarkId = Guid.NewGuid();
        private readonly Guid MockFolderId = Guid.NewGuid();
        private readonly string FakeUrl = "https://fakeurl.com";

        private Bookmark MockBookmarkEntity = null!;
        private BookmarkDTO MockBookmarkDTO = null!;
        private BookmarkCreationDTO MockBookmarkCreationDTO = null!;
        private BookmarkUpdateDTO MockBookmarkUpdateDTO = null!;

        IBookmarkRepository MockBookmarkRepository = null!;
        private BookmarkMapper MockBookmarkMapper = null!;

        IEnumerable<Bookmark> MockBookmarkList = null!;

        private BookmarkService bookmarkService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockBookmarkEntity = new("mockBookmark", FakeUrl, MockFolderId, MockUserId);

            MockBookmarkDTO = new BookmarkDTO
            {
                Id = MockBookmarkId,
                Name = "mockBookmark",
                Url = FakeUrl,
                IsMarkedFavorite = false,
                CreationDate = DateTime.UtcNow,
                LastVisitedTime = null
            };

            MockBookmarkCreationDTO = new BookmarkCreationDTO 
            {
                Name = "mockBookmark",
                Url = FakeUrl 
            };
            MockBookmarkUpdateDTO = new BookmarkUpdateDTO { Id = MockBookmarkId, Name = "mockBookmark" };

            MockBookmarkRepository = Substitute.For<IBookmarkRepository>();
            MockBookmarkMapper = new BookmarkMapper();

            MockBookmarkList = new List<Bookmark> { MockBookmarkEntity };

            bookmarkService = new(MockBookmarkRepository, MockBookmarkMapper);
        }

        // GetAllUserBookmarksAsync tests
        [TestMethod]
        public async Task GetAllBookmarksAsync_ReturnsIEnumerableBookmark()
        {
            //Arrange
            MockBookmarkRepository.GetAllByUserAndFolderIdAsync(MockUserId, MockFolderId)
                                  .Returns(MockBookmarkList);

            //Act
            var response = await bookmarkService.GetAllBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsInstanceOfType<IEnumerable<BookmarkDTO>>(response);
        }

        [TestMethod]
        public async Task GetAllBookmarksAsync_CallsGetAllAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.GetAllBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            await MockBookmarkRepository.Received(1).GetAllByUserAndFolderIdAsync(MockUserId, MockFolderId);
        }

        // GetUserBookmarkByIdAsync tests
        [TestMethod]
        public async Task GetBookmarkByIdAsync_ReturnsNull_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act
            var response = await bookmarkService.GetBookmarkByIdAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetBookmarkByIdAsync_ReturnsNull_WhenBookmarkDoesNotBelongToUser()
        {
            //Arrange
            Guid unauthorizedUserId = Guid.NewGuid();

            //Act 
            var result = await bookmarkService.GetBookmarkByIdAsync(unauthorizedUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetBookmarkByIdAsync_ReturnsBookmarkDTO()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            var result = await bookmarkService.GetBookmarkByIdAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsInstanceOfType<BookmarkDTO>(result);
        }

        // CreateUserBookmarkAsync tests
        [TestMethod]
        public async Task 
            CreateBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkCreationDTOIsNull()
        {
            //Arrange
            BookmarkCreationDTO nullBookmarkDTO = null!;

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
            bookmarkService.CreateBookmarkAsync(MockUserId, MockFolderId, nullBookmarkDTO));
        }

        [TestMethod]
        public async Task CreateBookmarkAsync_CallsAddAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.CreateBookmarkAsync(MockUserId, MockFolderId, MockBookmarkCreationDTO);

            //Assert
            await MockBookmarkRepository.Received(1).AddAsync(Arg.Any<Bookmark>());
        }

        // UpdateBookmarkAsync tests
        [TestMethod]
        public async Task 
            UpdateBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkUpdateDTOIsNull()
        {
            //Arrange
            BookmarkUpdateDTO nullBookmarkUpdateDTO = null!;

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
            bookmarkService.UpdateBookmarkAsync(MockUserId, MockFolderId, nullBookmarkUpdateDTO!));
        }

        [TestMethod]
        public async Task 
            UpdateBookmarkAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() => 
            bookmarkService.UpdateBookmarkAsync(MockUserId, MockFolderId, MockBookmarkUpdateDTO));
        }

        [TestMethod]
        public async Task UpdateBookmarkAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.UpdateBookmarkAsync(MockUserId, MockFolderId, MockBookmarkUpdateDTO);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(Arg.Any<Bookmark>());
        }

        // DeleteUserBookmarkAsync tests
        [TestMethod]
        public async Task 
            DeleteBookmarkAsync_ReturnsFalse_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act
            var result = await bookmarkService.DeleteBookmarkAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteBookmarkAsync_ReturnsTrue_WhenBookmarkIsDelete()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act 
            var result = await bookmarkService.DeleteBookmarkAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteBookmarkAsync_CallsDeleteAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.DeleteBookmarkAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).DeleteAsync(Arg.Any<Bookmark>());
        }

        // MarkUserBookmarkAsVisitedAsync tests
        [TestMethod]
        public async Task MarkBookmarkAsVisitedAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.MarkBookmarkAsVisitedAsync(MockUserId, MockFolderId, MockBookmarkId));
        }

        [TestMethod]
        public async Task MarkBookmarkAsVisitedAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.MarkBookmarkAsVisitedAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }
    }
}
