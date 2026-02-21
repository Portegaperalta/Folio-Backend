using Folio.Core.Application.DTOs.Pagination;
using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<IEnumerable<Bookmark>> GetAllByUserIdAsync(Guid userId, PaginationDTO paginationDTO);
        Task<IEnumerable<Bookmark>> GetAllByUserAndFolderIdAsync(Guid userId, Guid folderId, PaginationDTO paginationDTO);
        Task<Bookmark?> GetByIdAsync(Guid userId, Guid folderId, Guid bookmarkId);
        Task<Bookmark?> GetByIdAsNoTrackingAsync(Guid userId, Guid folderId, Guid bookmarkId);
        Task AddAsync(Bookmark bookmarkEntity);
        Task UpdateAsync(Bookmark bookmarkEntity);
        Task DeleteAsync(Bookmark bookmarkEntity);
        Task<bool> ExistsAsync(Guid userId, Guid bookmarkId);
        Task<int> CountByFolderAsync(Guid userId, Guid folderId);
        Task<int> CountByUserIdAsync(Guid userId);
        Task<Folder?> GetFolderByIdAsync(Guid folderId, Guid userId);
    }
}
