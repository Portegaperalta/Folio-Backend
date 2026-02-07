using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Folio_Backend_Tests.Core.Application.Services.UnitTests
{
    [TestClass]
    public class FolderServiceTests
    {
        private readonly Guid MockUserId = Guid.NewGuid();
        private readonly Guid MockFolderId = Guid.NewGuid();
        private readonly Guid MockInvalidUserId = Guid.NewGuid();
        private Folder MockFolderEntity = null!;
        private FolderDTO MockFolderDTO = null!;
        private IFolderRepository MockfolderRepository = null!;
        private FolderMapper MockFolderMapper = null!;
        private IEnumerable<Folder> MockFolderList = null!;
        private FolderService folderService = null!;

        [TestInitialize]
        public void Setup()
        {
            MockFolderEntity = new Folder("folderMock", MockUserId);

            MockfolderRepository = Substitute.For<IFolderRepository>();
            MockFolderMapper = new FolderMapper();

            MockFolderDTO = MockFolderMapper.ToDto(MockFolderEntity);


            MockFolderList = new List<Folder> { MockFolderEntity };

            folderService = new FolderService(MockfolderRepository, MockFolderMapper);
        }

        // GetAllFoldersAsync tests
        [TestMethod]
        public async Task GetAllFoldersAsync_ReturnsFolderDTOList()
        {
            //Arrange
            MockfolderRepository.GetAllAsync(MockUserId).Returns((MockFolderList));

            //Act
            var response = await folderService.GetAllFoldersAsync(MockUserId);

            //Assert
            Assert.IsInstanceOfType<IEnumerable<FolderDTO>>(response);
        }

        // GetFolderByIdAsync tests
        [TestMethod]
        public async Task 
            GetFolderByIdAsync_ReturnsNull_WhenFolderDoesNotExist()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.GetFolderByIdAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task 
            GetFolderByIdAsync_ReturnsNull_WhenFolderDoesNotBelongToUser()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockInvalidUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.GetFolderByIdAsync(MockInvalidUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetFolderByIdAsync_ReturnsFolderDTO()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            var result = await folderService.GetFolderByIdAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsInstanceOfType<FolderDTO>(result);
        }

        // CountFoldersAsync tests
        [TestMethod]
        public async Task CountFoldersAsync_ReturnsInteger()
        {
            //Arrange
            MockfolderRepository.CountByUserAsync(MockUserId).Returns(1);

            //Act
            var result = await folderService.CountFoldersAsync(MockUserId);

            //Assert
            Assert.IsInstanceOfType<int>(result);
        }

        [TestMethod]
        public async Task CountFolderAsync_CallsCountByUserAsyncFromFolderRepository()
        {
            //Act 
            await folderService.CountFoldersAsync(MockUserId);

            //Assert
            await MockfolderRepository.Received(1).CountByUserAsync(MockUserId);
        }

        // CreateFolder tests
        [TestMethod]
        public async Task CreateFolder_ThrowsArgumentNullException_WhenFolderEntityIsNull()
        {
            //Arrange
            Folder nullFolderEntity = null!;

            //Act + Assert
            var result = await Assert.ThrowsAsync <ArgumentNullException> (
                () => folderService.CreateFolder(nullFolderEntity));
        }

        [TestMethod]
        public async Task CreateFolder_CallsAddAsyncFromFolderRepository()
        {
            //Act
            await folderService.CreateFolder(MockFolderEntity);

            //Assert
            await MockfolderRepository.Received(1).AddAsync(MockFolderEntity);
        }

        // ChangeFolderNameAsync tests
        [TestMethod]
        public async Task
            ChangeFolderNameAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            string expectedExceptionMessage = $"Folder with id {MockFolderId} not found";
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            var result = await Assert.ThrowsAsync<FolderNotFoundException>(
                () => folderService.ChangeFolderNameAsync(MockUserId, MockFolderId, "newFolderName"));
        }

        // ChangeFolderNameAsync tests
        [TestMethod]
        public async Task ChangeFolderNameAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.ChangeFolderNameAsync(MockUserId, MockFolderId, "newFolderName");

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(MockFolderEntity);
        }

        // MarkFolderAsFavoriteAsync tests
        [TestMethod]
        public async Task 
            MarkFolderAsFavoriteAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            await Assert.ThrowsAsync<FolderNotFoundException>(
                () => folderService.MarkFolderAsFavoriteAsync(MockUserId, MockFolderId));
        }

        [TestMethod]
        public async Task MarkFolderAsFavoriteAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.MarkFolderAsFavoriteAsync(MockUserId, MockFolderId);

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(MockFolderEntity);
        }

        // UnmarkFolderAsFavoriteAsync tests
        [TestMethod]
        public async Task 
            UnmarkFolderAsFavoriteAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            await Assert.ThrowsAsync<FolderNotFoundException>(
                () => folderService.UnmarkFolderAsFavoriteAsync(MockUserId, MockFolderId));
        }

        [TestMethod]
        public async Task UnmarkFolderAsFavoriteAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.UnmarkFolderAsFavoriteAsync(MockUserId, MockFolderId);

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(MockFolderEntity);
        }

        // MarkUserFolderAsVisitedAsync tests
        [TestMethod]
        public async Task 
            MarkFolderAsVisitedAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            await Assert.ThrowsAsync<FolderNotFoundException>(
                () => folderService.MarkFolderAsVisitedAsync(MockUserId, MockFolderId));
        }

        [TestMethod]
        public async Task
            MarkFolderAsVisitedAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.MarkFolderAsVisitedAsync(MockUserId, MockFolderId);

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(MockFolderEntity);
        }

        // DeleteUserFolderAsync tests
        [TestMethod]
        public async Task 
            DeleteFolderAsync_ThrowsArgumentNullException_WhenFolderEntityIsNull()
        {
            //Arrange
            FolderDTO nullFolderEntity = null!;

            //Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => folderService.DeleteFolderAsync(MockUserId, nullFolderEntity));
        }

        [TestMethod]
        public async Task DeleteFolderAsync_CallsDeleteAsyncFromFolderRepository()
        {
            //Act
            await folderService.DeleteFolderAsync(MockUserId, MockFolderDTO);

            //Assert
            await MockfolderRepository.Received(1)
                                      .DeleteAsync(Arg.Any<Folder>());
        }
    }
}
