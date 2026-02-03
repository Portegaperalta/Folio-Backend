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
    }
}
