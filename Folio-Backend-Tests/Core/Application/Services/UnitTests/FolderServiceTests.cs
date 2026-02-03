using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Interfaces;
using NSubstitute;

namespace Folio_Backend_Tests.Core.Application.Services.UnitTests
{
    [TestClass]
    public class FolderServiceTests
    {
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly Guid MockFolderId = Guid.NewGuid();
        private readonly Guid MockInvalidUserId = Guid.NewGuid();
        private Folder MockFolderEntity = null!;
        IFolderRepository MockfolderRepository = null!;
        IEnumerable<Folder> MockFolderList = null!;
        private FolderService folderService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockFolderList = new List<Folder> { new Folder("folderMock", MockUserId) };
            MockfolderRepository = Substitute.For<IFolderRepository>();
            MockFolderEntity = new Folder("folderMock", MockUserId);
            folderService = new FolderService(MockfolderRepository);
        }

        // GetAllUserFoldersAsnyc tests
        [TestMethod]
        public async Task GetAllUserFoldersAsync_ReturnsIEnumerableFolder()
        {
            //Arrange
            MockfolderRepository.GetAllAsync(MockUserId).Returns(MockFolderList);

            //Act
            var response = await folderService.GetAllUserFoldersAsync(MockUserId);

            //Assert
            var result = response.ToList();
            CollectionAssert.AreEqual(expected: MockFolderList.ToList(), actual: result);
        }

        // GetUserFolderByIdAsyn tests
        [TestMethod]
        public async Task 
            GetUserFolderByIdAsync_ReturnsNull_WhenFolderDoesNotExist()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.GetUserFolderByIdAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task 
            GetUserFolderByIdAsync_ReturnsNull_WhenFolderDoesNotBelongToUser()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockInvalidUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.GetUserFolderByIdAsync(MockInvalidUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        // CreateUserFolder tests
        [TestMethod]
        public async Task CreateUserFolder_ThrowsArgumentNullException_WhenFolderEntityIsNull()
        {
            //Arrange
            Folder nullFolderEntity = null!;

            //Act + Assert
            var result = await Assert.ThrowsAsync <ArgumentNullException> (
                () => folderService.CreateUserFolder(nullFolderEntity));
        }

        [TestMethod]
        public async Task CreateUserFolder_CallsAddAsyncFromFolderRepository()
        {
            //Act
            await folderService.CreateUserFolder(MockFolderEntity);

            //Assert
            await MockfolderRepository.Received(1).AddAsync(MockFolderEntity);
        }

        // ChangeUserFolderNameAsync tests
        [TestMethod]
        public async Task
            ChangeUserFolderNameAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            string expectedExceptionMessage = $"Folder with id {MockFolderId} not found";
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            var result = await Assert.ThrowsAsync<FolderNotFoundException>(
                () => folderService.ChangeUserFolderNameAsync(MockUserId, MockFolderId, "newFolderName"));
        }

        // ChangeUserFolderNameAsync tests
        [TestMethod]
        public async Task ChangeUserFolderNameAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.ChangeUserFolderNameAsync(MockUserId, MockFolderId, "newFolderName");

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(MockFolderEntity);
        }
    }
}
