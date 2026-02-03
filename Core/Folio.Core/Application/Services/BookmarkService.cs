using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
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

        public async Task<IEnumerable<Bookmark>> GetAllUserBookmarksAsync(Guid userId, Guid folderId)
        {
            return await _bookmarkRepository.GetAllAsync(userId,folderId);
        }

        public async Task<Bookmark?> GetUserBookmarkByIdAsync(Guid userId, Guid folderId, Guid bookmarkId)
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
            ArgumentNullException.ThrowIfNull(bookmarkEntity);

            await _bookmarkRepository.AddAsync(bookmarkEntity!);
        }

        public async Task UpdateUserBookmarkAsync(Guid userId, Bookmark bookmarkEntity)
        {
            if (bookmarkEntity is null)
            {
                ArgumentNullException.ThrowIfNull("Bookmark entity cannot be null");
            }

            bool bookmarkExists = await _bookmarkRepository.ExistsAsync(userId,bookmarkEntity!.Id);

            if (bookmarkExists is false)
            {
                throw new BookmarkNotFoundException(bookmarkEntity.Id);
            }

            await _bookmarkRepository.UpdateAsync(bookmarkEntity);
        }

        public async Task DeleteUserBookmarkAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            await _bookmarkRepository.DeleteAsync(bookmark);
        }

        public async Task ChangeUserBookmarkName
            (Guid userId, Guid folderId, Guid bookmarkId, string newBookmarkName)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            bookmark.ChangeName(newBookmarkName);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task ChangeUserBookmarkUrl
            (Guid userId, Guid folderId, Guid bookmarkId, string newBookmarkUrl)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            bookmark.ChangeUrl(newBookmarkUrl);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkUserBookmarkAsFavorite(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            bookmark.MarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task UnmarkUserBookmarkAsFavorite(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            bookmark.UnmarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkUserBookmarkAsVisitedAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
            {
                throw new BookmarkNotFoundException(bookmarkId);
            }

            bookmark.Visit();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }
    }
}
