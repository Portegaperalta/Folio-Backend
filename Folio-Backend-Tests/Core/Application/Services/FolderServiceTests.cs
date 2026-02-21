using FluentAssertions;
using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.DTOs.Pagination;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.Folder;
using Folio.Core.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Folio_Backend_Tests.Core.Application.Services
{
    public class FolderServiceTests
    {
        private readonly IFolderRepository _mockfolderRepository;
        private readonly ICacheService _mockCacheService;
        private FolderMapper _folderMapper;
        private FolderService _folderService;

        public FolderServiceTests()
        {
            _mockfolderRepository = Substitute.For<IFolderRepository>();
            _mockCacheService = Substitute.For<ICacheService>();
            _folderMapper = new FolderMapper();
            _folderService = new FolderService(_mockfolderRepository, _folderMapper, _mockCacheService);
        }

        // GetAllFoldersAsync tests
        [Fact]
        public async Task GetAllFoldersAsync_ReturnsFolderDTOList()
        {
            //Arrange
            var userId = CreateUserId();
            var paginationDTO = new PaginationDTO();
            var folderEntity = CreateFolderEntity("mockFolder", userId);
            var folderList = CreateFolderList(folderEntity);

            _mockfolderRepository.GetAllAsync(userId, paginationDTO).Returns(folderList);

            //Act
            var response = await _folderService.GetAllFoldersDTOsAsync(userId, paginationDTO);

            //Assert
            response.Should().BeAssignableTo<IEnumerable<FolderDTO>>();
        }


        [Fact]
        public async Task GetAllFoldersAsync_CallsGetAllAsyncFromFolderRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var paginationDTO = new PaginationDTO();

            //Act
            await _folderService.GetAllFoldersDTOsAsync(userId, paginationDTO);

            //Assert
            await _mockfolderRepository.Received(1).GetAllAsync(userId, paginationDTO);
        }

        // GetFolderByIdAsync tests
        [Fact]
        public async Task
            GetFolderByIdAsync_ReturnsNull_WhenFolderDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns((Folder?)null);

            //Act
            var response = await _folderService.GetFolderDTOByIdAsync(userId, folderId);

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task
            GetFolderDTOByIdAsync_ReturnsNull_WhenFolderDoesNotBelongToUser()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                .Returns((Folder?)null);

            //Act
            var response = await _folderService.GetFolderDTOByIdAsync(userId, folderId);

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetFolderDTOByIdAsync_ReturnsFolderDTO()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var folderEntity = CreateFolderEntity("mockFolder", userId);

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                .Returns(folderEntity);

            //Act
            var result = await _folderService.GetFolderDTOByIdAsync(userId, folderId);

            //Assert
            result.Should().BeOfType<FolderDTO>();
        }

        // CountFoldersAsync tests
        [Fact]
        public async Task CountFoldersAsync_ReturnsInteger()
        {
            //Arrange
            var userId = CreateUserId();

            _mockfolderRepository.CountByUserAsync(userId).Returns(1);

            //Act
            var result = await _folderService.CountFoldersAsync(userId);

            //Assert
            result.Should().BeGreaterThanOrEqualTo(0);
            result.Should().Be(1);
        }

        [Fact]
        public async Task CountFolderAsync_CallsCountByUserAsyncFromFolderRepository()
        {
            //Arrange
            var userId = CreateUserId();

            //Act
            await _folderService.CountFoldersAsync(userId);

            //Assert
            await _mockfolderRepository.Received(1).CountByUserAsync(userId);
        }

        // CreateFolder tests
        [Fact]
        public async Task CreateFolderAsync_ThrowsArgumentNullException_WhenFolderEntityIsNull()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            FolderCreationDTO nullFolderCreationDTO = null!;

            //Act 
            Func<Task> act = async () => await _folderService.CreateFolderAsync(userId, nullFolderCreationDTO);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateFolderAsync_CallsAddAsyncFromFolderRepository()
        {
            //Act
            var userId = CreateUserId();

            var folderCreationDTO = CreateFolderCreationDTO("mockFolder");

            await _folderService.CreateFolderAsync(userId, folderCreationDTO);

            //Assert
            await _mockfolderRepository.Received(1).AddAsync(Arg.Any<Folder>());
        }

        // UpdateFolderAsync tests
        [Fact]
        public async Task
            UpdateFolderAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange 
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns((Folder?)null);

            var folderUpdateDTO = CreateFolderUpdateDTO("mockFolderUpdate", true);

            //Act 
            Func<Task> act = async () => await _folderService.UpdateFolderAsync(userId, folderId, folderUpdateDTO);

            //Assert
            await act.Should().ThrowAsync<FolderNotFoundException>();
        }

        [Fact]
        public async Task UpdateFolderAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var folderEntity = CreateFolderEntity("mockUser", userId);

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns(folderEntity);

            var folderUpdateDTO = CreateFolderUpdateDTO("mockFolderUpdate", true);

            //Act
            await _folderService.UpdateFolderAsync(folderId, userId, folderUpdateDTO);

            //Assert
            await _mockfolderRepository.Received(1).UpdateAsync(Arg.Any<Folder>());
        }

        // MarkUserFolderAsVisitedAsync tests
        [Fact]
        public async Task
            MarkFolderAsVisitedAsync_ThrowsFolderNotFoundException_WhenFolderDoesNotExist()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns((Folder?)null);

            var folderUpdateDTO = CreateFolderUpdateDTO("mockFolderUpdate");

            //Act 
            Func<Task> act = async () => await _folderService.UpdateFolderAsync(userId, folderId, folderUpdateDTO);

            //Assert
            await act.Should().ThrowAsync<FolderNotFoundException>();
        }

        [Fact]
        public async Task
            MarkFolderAsVisitedAsync_CallsUpdateAsyncFromFolderRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var folderEntity = CreateFolderEntity("mockUser", userId);


            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns(folderEntity);

            //Act
            await _folderService.MarkFolderAsVisitedAsync(userId, folderId);

            //Assert
            await _mockfolderRepository.Received(1).UpdateAsync(folderEntity);
        }

        // DeleteFolderAsync tests
        [Fact]
        public async Task
            DeleteFolderAsync_ReturnsFalse_WhenFolderDoesNotExists()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns((Folder?)null);

            //Act
            var response = await _folderService.DeleteFolderAsync(userId, folderId);

            //Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteFolderAsync_ReturnsTrue_WhenFolderIsDeleted()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var folderEntity = CreateFolderEntity("mockUser", userId);

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                 .Returns(folderEntity);

            //Act
            var result = await _folderService.DeleteFolderAsync(userId, folderId);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteFolderAsync_CallsDeleteAsyncFromFolderRepository()
        {
            //Arrange
            var userId = CreateUserId();
            var folderId = CreateFolderId();
            var folderEntity = CreateFolderEntity("mockUser", userId);

            _mockfolderRepository.GetByIdAsync(userId, folderId)
                                .Returns(folderEntity);

            //Act
            await _folderService.DeleteFolderAsync(userId, folderId);

            //Assert
            await _mockfolderRepository.Received(1).DeleteAsync(Arg.Any<Folder>());
        }

        // Helper methods
        private Guid CreateUserId() => new();

        private Guid CreateFolderId() => new();

        private Folder CreateFolderEntity(string name, Guid userId)
        {
            return new Folder(name, userId);
        }

        private FolderDTO CreateFolderDTO(string name, Guid folderId, DateTime creationDate,
            bool isMarkedFavorite)
        {
            return new FolderDTO
            {
                Id = folderId,
                Name = name,
                CreationDate = creationDate,
                IsMarkedFavorite = isMarkedFavorite
            };
        }

        private FolderCreationDTO CreateFolderCreationDTO(string name)
        {
            return new FolderCreationDTO { Name = name };
        }

        private FolderUpdateDTO CreateFolderUpdateDTO(string name, bool isMarkedFavorite = false)
        {
            return new FolderUpdateDTO
            {
                Name = name,
                IsMarkedFavorite = isMarkedFavorite
            };
        }

        private IEnumerable<Folder> CreateFolderList(Folder folderEntity)
        {
            return new List<Folder> { folderEntity };
        }
    }
}


