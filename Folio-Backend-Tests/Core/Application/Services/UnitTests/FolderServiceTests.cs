using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.Folder;
using Folio.Core.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System.Security.Cryptography.X509Certificates;

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
        private FolderCreationDTO MockFolderCreationDTO = null!;
        private FolderUpdateDTO MockFolderUpdateDTO = null!;

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
            MockFolderCreationDTO = new FolderCreationDTO { Name = "folderMock" };
            MockFolderUpdateDTO = new FolderUpdateDTO { Name = "folderMock" };

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
            var response = await folderService.GetAllFoldersDTOsAsync(MockUserId);

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
            var response = await folderService.GetFolderDTOByIdAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task 
            GetFolderDTOByIdAsync_ReturnsNull_WhenFolderDoesNotBelongToUser()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockInvalidUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.GetFolderDTOByIdAsync(MockInvalidUserId, MockFolderId);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetFolderDTOByIdAsync_ReturnsFolderDTO()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            var result = await folderService.GetFolderDTOByIdAsync(MockUserId, MockFolderId);

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
        public async Task CreateFolderAsync_ThrowsArgumentNullException_WhenFolderEntityIsNull()
        {
            //Arrange
            FolderCreationDTO nullFolderEntity = null!;

            //Act + Assert
            var result = await Assert.ThrowsAsync <ArgumentNullException> (
                () => folderService.CreateFolderAsync(MockUserId ,nullFolderEntity));
        }

        [TestMethod]
        public async Task CreateFolderAsync_CallsAddAsyncFromFolderRepository()
        {
            //Act
            await folderService.CreateFolderAsync(MockUserId, MockFolderCreationDTO);

            //Assert
            await MockfolderRepository.Received(1).AddAsync(Arg.Any<Folder>());
        }

        // UpdateFolderAsync tests
        [TestMethod]
        public async Task 
            UpdateFolderAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange 
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act + Assert
            await Assert.ThrowsAsync<FolderNotFoundException>(() =>
            folderService.UpdateFolderAsync(MockFolderId, MockUserId, MockFolderUpdateDTO));
        }

        [TestMethod]
        public async Task UpdateFolderAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.UpdateFolderAsync(MockFolderId, MockUserId, MockFolderUpdateDTO);

            //Assert
            await MockfolderRepository.Received(1).UpdateAsync(Arg.Any<Folder>());
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

        // DeleteFolderAsync tests
        [TestMethod]
        public async Task 
            DeleteFolderAsync_ReturnsFalse_WhenFolderDoesNotExists()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns((Folder?)null);

            //Act
            var response = await folderService.DeleteFolderAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task DeleteFolderAsync_ReturnsTrue_WhenFolderIsDeleted()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            var result = await folderService.DeleteFolderAsync(MockUserId, MockFolderId);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteFolderAsync_CallsDeleteAsyncFromFolderRepository()
        {
            //Arrange
            MockfolderRepository.GetByIdAsync(MockUserId, MockFolderId)
                                .Returns(MockFolderEntity);

            //Act
            await folderService.DeleteFolderAsync(MockUserId, MockFolderId);

            //Assert
            await MockfolderRepository.Received(1)
                                      .DeleteAsync(Arg.Any<Folder>());
        }
    }
}
