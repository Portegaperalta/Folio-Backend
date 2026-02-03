using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
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
        IBookmarkRepository MockBookmarkRepository = null!;
        IEnumerable<Bookmark> MockBookmarkList = null!;
        private BookmarkService bookmarkService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockBookmarkEntity = new("mockBookmark", FakeUrl, MockFolderId, MockUserId);
            MockBookmarkRepository = Substitute.For<IBookmarkRepository>();
            MockBookmarkList = new List<Bookmark> { MockBookmarkEntity };
            bookmarkService = new(MockBookmarkRepository);
        }

        [TestMethod]
        public async Task GetAllUserBookmarksAsync_ReturnsIEnumerableBookmark()
        {
            //Arrange
            MockBookmarkRepository.GetAllAsync(MockUserId, MockFolderId)
                                  .Returns(MockBookmarkList);

            //Act
            var response = await bookmarkService.GetAllUserBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            var result = response.ToList();
            CollectionAssert.AreEqual(expected: MockBookmarkList.ToList(), actual: result);
        }

        [TestMethod]
        public async Task GetAllUserBookmarksAsync_CallsGetAllAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.GetAllUserBookmarksAsync(MockUserId, MockFolderId);

            //Assert
            await MockBookmarkRepository.Received(1).GetAllAsync(MockUserId, MockFolderId);
        }

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
        public async Task GetUserBookmarkByIdAsync_ReturnsBookmarkEntity()
        {
            //Arrange
            MockBookmarkRepository.GetByIdAsync(MockUserId, MockFolderId, MockBookmarkId)
                                  .Returns(MockBookmarkEntity);

            //Act
            var result = await bookmarkService.GetUserBookmarkByIdAsync(MockUserId, MockFolderId, MockBookmarkId);

            //Assert
            Assert.AreEqual(expected: MockBookmarkEntity, actual: result);
        }

        [TestMethod]
        public async Task CreateUserBookmarkAsync_ThrowsArgumentNullException_WhenBookmarkEntityIsNull()
        {
            //Arrange
            Bookmark nullBookmarkEntity = null!;

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
            bookmarkService.CreateUserBookmarkAsync(nullBookmarkEntity));
        }

        [TestMethod]
        public async Task CreateUserBookmarkAsync_CallsAddAsyncFromBookmarkRepository()
        {
            //Act
            await bookmarkService.CreateUserBookmarkAsync(MockBookmarkEntity);

            //Assert
            await MockBookmarkRepository.Received(1).AddAsync(MockBookmarkEntity);
        }
    }
}
