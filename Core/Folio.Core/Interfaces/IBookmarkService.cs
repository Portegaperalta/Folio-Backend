using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.DTOs.Pagination;

namespace Folio.Core.Interfaces
{
    public interface IBookmarkService
    {
        Task<IEnumerable<BookmarkDTO>> GetAllBookmarksAsync(Guid userId, Guid? folderId, PaginationDTO paginationDTO);
        Task<BookmarkDTO?> GetBookmarkByIdAsync(Guid userId, Guid folderId, Guid bookmarkId);
        Task<int> CountBookmarksByFolderIdAsync(Guid userId, Guid folderId);
        Task<BookmarkDTO> CreateBookmarkAsync(Guid userId, Guid folderId, BookmarkCreationDTO bookmarkCreationDTO);
        Task UpdateBookmarkAsync(Guid userId, Guid folderId, BookmarkUpdateDTO bookmarkUpdateDTO);
        Task<bool> DeleteBookmarkAsync(Guid userId, Guid folderId, Guid bookmarkId);
        Task MarkBookmarkAsVisitedAsync(Guid userId, Guid folderId, Guid bookmarkId);
    }
}
