using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<IEnumerable<Bookmark>> GetAllAsync(Guid userId,int folderId);
        Task<Bookmark?> GetByIdAsync(Guid userId, int folderId,Guid bookmarkId);
        Task<Bookmark?> GetByIdAsNoTrackingAsync(Guid userId, int folderId, Guid bookmarkId);
        Task AddAsync(Bookmark bookmarkEntity);
        Task UpdateAsync(Bookmark bookmarkEntity);
        Task DeleteAsync(Bookmark bookmarkEntity);
        Task<bool> ExistsAsync(Guid userId, Guid bookmarkId);
        Task<int> CountByFolderAsync(Guid userId, int folderId);
    }
}
