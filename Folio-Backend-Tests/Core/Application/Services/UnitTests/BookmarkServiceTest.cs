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

        IBookmarkRepository MockBookmarkRepository = null!;
        private BookmarkMapper MockBookmarkMapper = null!;

        IEnumerable<Bookmark> MockBookmarkList = null!;
        private IEnumerable<BookmarkDTO> MockBookmarkDTOList = null!;

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

            MockBookmarkRepository = Substitute.For<IBookmarkRepository>();
            MockBookmarkMapper = new BookmarkMapper();

            MockBookmarkList = new List<Bookmark> { MockBookmarkEntity };
            MockBookmarkDTOList = new List<BookmarkDTO> { MockBookmarkDTO };

            bookmarkService = new(MockBookmarkRepository, MockBookmarkMapper);
        }

        // GetAllUserBookmarksAsync tests
        [TestMethod]
        public async Task GetAllUserBookmarksAsync_ReturnsIEnumerableBookmark()
        {
            //Arrange
            MockBookmarkRepository.GetAllByUserAndFolderIdAsync(MockUserId, MockFolderId)
                                  .Returns(MockBookmarkList);

            //Act
            var response = await bookmarkService.GetAllUserBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsInstanceOfType<IEnumerable<BookmarkDTO>>(response);
        }

        [TestMethod]
        public async Task GetAllUserBookmarksAsync_CallsGetAllAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.GetAllUserBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            await MockBookmarkRepository.Received(1).GetAllByUserAndFolderIdAsync(MockUserId, MockFolderId);
        }

        // GetUserBookmarkByIdAsync tests
        [TestMethod]
        public async Task GetUserBookmarkByIdAsync_ReturnsNull_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act
            var response = await bookmarkService.GetUserBookmarkByIdAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetUserFolderByIdAsync_ReturnsNull_WhenBookmarkDoesNotBelongToUser()
        {
            //Arrange
            Guid unauthorizedUserId = Guid.NewGuid();

            //Act 
            var result = await bookmarkService.GetUserBookmarkByIdAsync(unauthorizedUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserBookmarkByIdAsync_ReturnsBookmarkDTO()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            var result = await bookmarkService.GetUserBookmarkByIdAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.IsInstanceOfType<BookmarkDTO>(result);
        }

        // CreateUserBookmarkAsync tests
        [TestMethod]
        public async Task 
            CreateUserBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkCreationDTOIsNull()
        {
            //Arrange
            BookmarkCreationDTO nullBookmarkDTO = null!;

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
            bookmarkService.CreateUserBookmarkAsync(MockUserId, MockFolderId, nullBookmarkDTO));
        }

        [TestMethod]
        public async Task CreateUserBookmarkAsync_CallsAddAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.CreateUserBookmarkAsync(MockUserId, MockFolderId, MockBookmarkCreationDTO);

            //Assert
            await MockBookmarkRepository.Received(1).AddAsync(Arg.Any<Bookmark>());
        }

        // DeleteUserBookmarkAsync tests
        [TestMethod]
        public async Task 
            DeleteUserBookmarkAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.DeleteUserBookmarkAsync(MockUserId, MockFolderId, MockBookmarkId));
        }

        [TestMethod]
        public async Task DeleteUserBookmarkAsync_CallsDeleteAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.DeleteUserBookmarkAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).DeleteAsync(MockBookmarkEntity);
        }

        // ChangeUserBookmarkNameAsync tests
        [TestMethod]
        public async Task 
            ChangeUserBookmarkNameAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.ChangeUserBookmarkNameAsync(MockUserId, MockFolderId, MockBookmarkId, "newName"));
        }

        [TestMethod]
        public async Task ChangeUserBookmarkNameAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.ChangeUserBookmarkNameAsync(MockUserId, MockFolderId, MockBookmarkId, "newName");

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }

        // ChangeUserBookmarkUrlAsync tests
        [TestMethod]
        public async Task 
            ChangeUserBookmarkUrlAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.ChangeUserBookmarkUrlAsync(MockUserId, MockFolderId, MockBookmarkId, FakeUrl));
        }

        [TestMethod]
        public async Task ChangeUserBookmarkUrlAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.ChangeUserBookmarkUrlAsync(MockUserId, MockFolderId, MockBookmarkId, FakeUrl);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }

        //MarkUserBookmarkAsFavoriteAsync tests
        [TestMethod]
        public async Task MarkUserBookmarkAsFavoriteAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.MarkUserBookmarkAsFavoriteAsync(MockUserId, MockFolderId, MockBookmarkId));
        }

        [TestMethod]
        public async Task MarkUserBookmarkAsFavoriteAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.MarkUserBookmarkAsFavoriteAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }

        // UnmarkUserBookmarkAsFavoriteAsync tests
        [TestMethod]
        public async Task UnmarkUserBookmarkAsFavoriteAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.UnmarkUserBookmarkAsFavoriteAsync(MockUserId, MockFolderId, MockBookmarkId));
        }

        [TestMethod]
        public async Task UnmarkUserBookmarkAsFavoriteAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.UnmarkUserBookmarkAsFavoriteAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }

        // MarkUserBookmarkAsVisitedAsync tests
        [TestMethod]
        public async Task MarkUserBookmarkAsVisitedAsync_ThrowsBookmarkNotFoundException_WhenBookmarkDoesNotExist()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns((Bookmark?)null);

            //Act + Assert
            await Assert.ThrowsAsync<BookmarkNotFoundException>(() =>
            bookmarkService.MarkUserBookmarkAsVisitedAsync(MockUserId, MockFolderId, MockBookmarkId));
        }

        [TestMethod]
        public async Task MarkUserBookmarkAsVisitedAsync_CallsUpdateAsyncFromBookmarkRepository()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            await bookmarkService.MarkUserBookmarkAsVisitedAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            await MockBookmarkRepository.Received(1).UpdateAsync(MockBookmarkEntity);
        }
    }
}
