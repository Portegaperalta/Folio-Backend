using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.Mappers;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class FolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly FolderMapper _folderMapper;

        public FolderService(IFolderRepository folderRepository, FolderMapper folderMapper)
        {
            _folderRepository = folderRepository;
            _folderMapper = folderMapper;
        }

        public async Task<IEnumerable<FolderDTO>> GetAllFoldersAsync(Guid userId)
        {
            var folders = await _folderRepository.GetAllAsync(userId);

            var foldersDTOs = folders.Select(f => _folderMapper.ToDto(f));

            return foldersDTOs;
        }

        public async Task<FolderDTO?> GetFolderByIdAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                return null;

            var folderDTO = _folderMapper.ToDto(folder);

            return folderDTO;
        }

        public async Task<int> CountFolders(Guid userId)
        {
            return await _folderRepository.CountByUserAsync(userId);
        }

        public async Task CreateFolder(Folder folderEntity)
        {
            ArgumentNullException.ThrowIfNull(folderEntity);

            await _folderRepository.AddAsync(folderEntity);
        }

        public async Task ChangeFolderNameAsync(Guid userId, Guid folderId, string newFolderName)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                throw new FolderNotFoundException(folderId);

            folder.ChangeName(newFolderName);

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkFolderAsFavoriteAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                throw new FolderNotFoundException(folderId);

            folder.MarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task UnmarkFolderAsFavoriteAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                throw new FolderNotFoundException(folderId);

            folder.UnmarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkFolderAsVisitedAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                throw new FolderNotFoundException(folderId);

            folder.Visit();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task DeleteFolderAsync(Guid userId, Folder folderEntity)
        {
            if (folderEntity is null)
                ArgumentNullException.ThrowIfNull(folderEntity);

            await _folderRepository.DeleteAsync(folderEntity);
        }
    }
}
