using Folio.Core.Domain;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class BookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkService(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<IEnumerable<Bookmark>> GetAllUserBookmarksAsync(int folderId)
        {
            return await _bookmarkRepository.GetAllAsync(folderId);
        }

        public async Task<Bookmark?> GetUserBookmarkByIdAsync(int userId, int folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
            {
                return null;
            }

            return bookmark;
        }

        public async Task CreateUserBookmarkAsync(Bookmark bookmarkEntity)
        {
            if (bookmarkEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Bookmark entity cannot be null");
            }

            await _bookmarkRepository.AddAsync(bookmarkEntity!);
        }

        public async Task UpdateUserBookmarkAsync(Bookmark bookmarkEntity)
        {
            if (bookmarkEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Bookmark entity cannot be null");
            }

            bool bookmarkExists = await _bookmarkRepository.ExistsAsync(bookmarkEntity!.Id);

            if (bookmarkExists is false)
            {
                throw new ArgumentException($"Bookmark with id:{bookmarkEntity.Id} not found");
            }

            await _bookmarkRepository.UpdateAsync(bookmarkEntity);
        }

        public async Task DeleteUserBookmarkAsync(int userId, int folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            await _bookmarkRepository.DeleteAsync(bookmark);
        }

        public async Task ChangeUserBookmarkName
            (int userId, int folderId, Guid bookmarkId, string newBookmarkName)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            bookmark.ChangeName(newBookmarkName);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task ChangeUserBookmarkUrl
            (int userId, int folderId, Guid bookmarkId, string newBookmarkUrl)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            bookmark.ChangeUrl(newBookmarkUrl);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkUserBookmarkAsFavorite(int userId, int folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            bookmark.MarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task UnmarkUserBookmarkAsFavorite(int userId, int folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            bookmark.UnmarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkUserBookmarkAsVisitedAsync(int userId, int folderId,Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
            {
                throw new ArgumentException($"Bookmark with id {bookmarkId} not found");
            }

            bookmark.Visit();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }
    }
}
