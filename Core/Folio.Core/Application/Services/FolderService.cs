using Folio.Core.Domain;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class FolderService
    {
        private readonly IFolderRepository _folderRepository;

        public FolderService(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<IEnumerable<Folder>> GetAllUserFoldersAsync(Guid userId)
        {
            var folders = await _folderRepository.GetAllAsync(userId);
            return folders;
        }

        public async Task<Folder?> GetUserFolderByIdAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                return null;
            }

            return folder;
        }

        public async Task CreateUserFolder(Folder folderEntity)
        {
            ArgumentNullException.ThrowIfNull(folderEntity);

            await _folderRepository.AddAsync(folderEntity);
        }

        public async Task ChangeUserFolderNameAsync(Guid userId, Guid folderId, string newFolderName)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id {folderId} not found");
            }

            folder.ChangeName(newFolderName);

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkUserFolderAsFavoriteAync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            folder.MarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task UnmarkUserFolderAsFavoriteAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            folder.UnmarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkUserFolderAsVisitedAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            folder.Visit();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task DeleteUserFolderAsync(Guid userId, Folder folderEntity)
        {
            if (folderEntity is null)
            {
                ArgumentNullException.ThrowIfNull(folderEntity);
            }

            await _folderRepository.DeleteAsync(folderEntity);
        }
    }
}
