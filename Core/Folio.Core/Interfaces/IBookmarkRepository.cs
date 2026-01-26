using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<IEnumerable<Bookmark>> GetAllAsync(int userId,int folderId);
        Task<Bookmark?> GetByIdAsync(int userId,int folderId,Guid bookmarkId);
        Task AddAsync(Bookmark bookmarkEntity);
        Task UpdateAsync(Bookmark bookmarkEntity);
        Task DeleteAsync(Bookmark bookmarkEntity);
        Task<bool> ExistsAsync(int userId,Guid bookmarkId);
        Task<int> CountByFolderAsync(int userId,int folderId);
    }
}
