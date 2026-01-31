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

        public async Task<IEnumerable<Folder>> GetAllUserFoldersAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must higher than 0");
            }

            var folders = await _folderRepository.GetAllAsync(userId);
            return folders;
        }

        public async Task<Folder?> GetUserFolderByIdAsync(int userId,int folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                return null;
            }

            if (folder.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User with id: {userId} does not have access to folder with id: {folderId}");
            }

            return folder;
        }

        public async Task CreateUserFolder(Folder folderEntity)
        {
            if (folderEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Folder entity cannot be null");
            }

            await _folderRepository.AddAsync(folderEntity!);
        }

        public async Task UpdateUserFolderAsync(int userId,Folder folderEntity)
        {
            if (folderEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Folder entity cannot be null");
            }

            var folderExists = await _folderRepository.ExistsAsync(userId,folderEntity!.Id);

            if (folderExists is false)
            {
                throw new ArgumentException($"Folder with id:{folderEntity.Id} not found");
            }

            if (folderEntity.UserId != userId)
            {
                throw new UnauthorizedAccessException
                    ($"User with id: {userId} does not have access to folder with id: {folderEntity.Id}");
            }

            await _folderRepository.UpdateAsync(folderEntity);
        }

        public async Task DeleteUserFolderAsync(int userId,Folder folderEntity)
        {
            bool folderExists = await _folderRepository.ExistsAsync(userId,folderEntity.Id);

            if (folderExists is false)
            {
                throw new ArgumentException($"Folder with id {folderEntity.Id} not found");
            }

            if (folderEntity.UserId != userId)
            {
                throw new UnauthorizedAccessException
                    ($"User with id: {userId} does not have access to folder with id: {folderEntity.Id}");
            }

            await _folderRepository.DeleteAsync(folderEntity);
        }

        public async Task ChangeUserFolderNameAsync(int userId,int folderId, string newFolderName)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id {folderId} not found");
            }

            if (folder.UserId != userId)
            {
                throw new UnauthorizedAccessException
                    ($"User with id: {userId} does not have access to folder with id: {folderId}");
            }

            folder.ChangeName(newFolderName);

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkUserFolderAsFavoriteAync(int userId, int folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            if (folder.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User with id: {userId} does not have access to folder with id: {folderId}");
            }

            folder.MarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task UnmarkUserFolderAsFavoriteAsync(int userId, int folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            if (folder.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User with id: {userId} does not have access to folder with id: {folderId}");
            }

            folder.UnmarkFavorite();

            await _folderRepository.UpdateAsync(folder);
        }

        public async Task MarkUserFolderAsVisitedAsync(int userId,int folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
            {
                throw new ArgumentException($"Folder with id: {folderId} not found");
            }

            if (folder.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User with id: {userId} does not have access to folder with id: {folderId}");
            }

            folder.Visit();

            await _folderRepository.UpdateAsync(folder);
        }
    }
}
