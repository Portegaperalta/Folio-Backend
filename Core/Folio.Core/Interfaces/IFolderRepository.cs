using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IFolderRepository
    {
        Task<IEnumerable<Folder>> GetAllAsync(Guid userId);
        Task<Folder?> GetByIdAsync(Guid userId, int folderId);
        Task AddAsync(Folder folderEntity);
        Task UpdateAsync(Folder folderEntity);
        Task DeleteAsync(Folder folderEntity);
        Task<int> CountByUserAsync(Guid userId);
    }
}
