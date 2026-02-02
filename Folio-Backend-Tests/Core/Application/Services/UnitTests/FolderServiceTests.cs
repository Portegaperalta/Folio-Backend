using Folio.Core.Application.Services;
using Folio.Core.Domain;
using Folio.Core.Interfaces;
using NSubstitute;

namespace Folio_Backend_Tests.Core.Application.Services.UnitTests
{
    [TestClass]
    public class FolderServiceTests
    {
        private Guid MockUserId = Guid.NewGuid();
        private Guid MockFolderId = Guid.NewGuid();
        private Folder MockFolderEntity = null!;
        IFolderRepository MockfolderRepository = null!;
        IEnumerable<Folder> MockFolderList = null!;
        private FolderService folderService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockFolderList = new List<Folder> { new Folder("folderMock", MockUserId) };
            MockfolderRepository = Substitute.For<IFolderRepository>();
            MockFolderEntity = Substitute.For<Folder>("folderMock",MockUserId);
            folderService = new FolderService(MockfolderRepository);
        }

        [TestMethod]
        public async Task GetAllUserFoldersAsync_ReturnsIEnumerableFolder()
        {
            //Arrange
            MockfolderRepository.GetAllAsync(MockUserId).Returns(MockFolderList);

            //Act
            var response = await folderService.GetAllUserFoldersAsync(MockUserId);
            var result = response.ToList();

            //Assert
            CollectionAssert.AreEqual(expected: MockFolderList.ToList(), actual: result);
        }
    }
}
