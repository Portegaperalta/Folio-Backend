using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IFolderRepository
    {
        Task<IEnumerable<Folder>> GetAllAsync(int userId);
        Task<Folder?> GetByIdAsync(int userId,int folderId);
        Task AddAsync(Folder folderEntity);
        Task UpdateAsync(Folder folderEntity);
        Task DeleteAsync(Folder folderEntity);
        Task<bool> ExistsAsync(int folderId);
        Task<int> CountByUserAsync(int userId);
    }
}
