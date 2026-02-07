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

        public async Task<IEnumerable<FolderDTO>> GetAllFoldersDTOsAsync(Guid userId)
        {
            var folders = await _folderRepository.GetAllAsync(userId);

            var foldersDTOs = folders.Select(f => _folderMapper.ToDto(f));

            return foldersDTOs;
        }

        public async Task<FolderDTO?> GetFolderDTOByIdAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                return null;

            var folderDTO = _folderMapper.ToDto(folder);

            return folderDTO;
        }

        public async Task<int> CountFoldersAsync(Guid userId)
        {
            return await _folderRepository.CountByUserAsync(userId);
        }

        public async Task<FolderDTO> CreateFolderAsync(Guid userId, FolderCreationDTO folderCreationDTO)
        {
            ArgumentNullException.ThrowIfNull(folderCreationDTO);

            var folderEntity = _folderMapper.ToEntity(userId, folderCreationDTO);

            await _folderRepository.AddAsync(folderEntity);

            var folderDTO = _folderMapper.ToDto(folderEntity);

            return folderDTO;
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

        public async Task<bool> DeleteFolderAsync(Guid userId, Guid folderId)
        {
            var folderEntity = await GetFolderEntityByIdAsync(userId, folderId);

            if (folderEntity is null)
            {
                return false;
            }    

            await _folderRepository.DeleteAsync(folderEntity);

            return true;
        }

        private async Task<Folder?> GetFolderEntityByIdAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                return null;
            }

            return folder;
        }
    }
}
