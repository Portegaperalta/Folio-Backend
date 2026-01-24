using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<IEnumerable<Bookmark>> GetAllAsync(int folderId);
        Task<Bookmark?> GetByIdAsync(Guid bookmarkId);
        Task AddAsync(Bookmark bookmarkEntity);
        Task UpdateAsync(Bookmark bookmarkEntity);
        Task DeleteAsync(Bookmark bookmarkEntity);
        Task<bool> ExistsAsync(Guid bookmarkId);
        Task<int> CountByFolderAsync(int folderId);
    }
}
