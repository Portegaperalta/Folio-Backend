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

        public async Task UpdateUserFolderAsync(Folder folderEntity)
        {
            if (folderEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Folder entity cannot be null");
            }

            var folderExists = await _folderRepository.ExistsAsync(folderEntity!.Id);

            if (folderExists is false)
            {
                throw new NullReferenceException($"Folder with id:{folderEntity.Id} not found");
            }

            await _folderRepository.UpdateAsync(folderEntity);
        }

        public async Task DeleteUserFolderAsync(Folder folderEntity)
        {
            bool folderExists = await _folderRepository.ExistsAsync(folderEntity.Id);

            if (folderExists is false)
            {
                throw new ArgumentException($"Folder with id {folderEntity.Id} not found");
            }

            await _folderRepository.DeleteAsync(folderEntity);
        }
    }
}
