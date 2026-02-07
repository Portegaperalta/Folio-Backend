using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.Mappers;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class BookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly BookmarkMapper _bookmarkMapper;

        public BookmarkService(IBookmarkRepository bookmarkRepository, BookmarkMapper bookmarkMapper)
        {
            _bookmarkRepository = bookmarkRepository;
            _bookmarkMapper = bookmarkMapper;
        }

        public async Task<IEnumerable<BookmarkDTO>> GetAllBookmarksAsync(Guid userId, Guid? folderId)
        {
            if (folderId is null)
            {
                var bookmarksByUserId = await _bookmarkRepository.GetAllByUserIdAsync(userId);

                var bookmarksByUserIdDTOs = bookmarksByUserId.Select(b => _bookmarkMapper.ToDto(b));

                return bookmarksByUserIdDTOs;
            }

            var bookmarks = await _bookmarkRepository.GetAllByUserAndFolderIdAsync(userId, folderId.Value);

            var bookmarksDTO = bookmarks.Select(b => _bookmarkMapper.ToDto(b));

            return bookmarksDTO;
        }

        public async Task<BookmarkDTO?> GetBookmarkByIdAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
                return null;

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmark);

            return bookmarkDTO;
        }

        public async Task<int> CountBookmarksByFolderIdAsync(Guid userId, Guid folderId)
        {
            return await _bookmarkRepository.CountByFolderAsync(userId, folderId);
        }

        public async Task<BookmarkDTO> CreateBookmarkAsync(Guid userId, Guid folderId, BookmarkCreationDTO bookmarkCreationDTO)
        {
            ArgumentNullException.ThrowIfNull(bookmarkCreationDTO);

            var bookmarkEntity = _bookmarkMapper.ToEntity(userId, folderId, bookmarkCreationDTO);

            await _bookmarkRepository.AddAsync(bookmarkEntity);

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmarkEntity);

            return bookmarkDTO;
        }

        public async Task UpdateBookmarkAsync(Guid userId, Bookmark bookmarkEntity)
        {
            ArgumentNullException.ThrowIfNull(bookmarkEntity);

            bool bookmarkExists = await _bookmarkRepository.ExistsAsync(userId,bookmarkEntity!.Id);

            if (bookmarkExists is false)
                throw new BookmarkNotFoundException(bookmarkEntity.Id);

            await _bookmarkRepository.UpdateAsync(bookmarkEntity);
        }

        public async Task<bool> DeleteBookmarkAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmarkEntity = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmarkEntity is null)
            {
                return false;
            }

            await _bookmarkRepository.DeleteAsync(bookmarkEntity);

            return true;
        }

        public async Task ChangeBookmarkNameAsync
            (Guid userId, Guid folderId, Guid bookmarkId, string newBookmarkName)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.ChangeName(newBookmarkName);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task ChangeBookmarkUrlAsync
            (Guid userId, Guid folderId, Guid bookmarkId, string newBookmarkUrl)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.ChangeUrl(newBookmarkUrl);

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkBookmarkAsFavoriteAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.MarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task UnmarkBookmarkAsFavoriteAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.UnmarkFavorite();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }

        public async Task MarkBookmarkAsVisitedAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId,folderId,bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            if (bookmark.UserId != userId)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.Visit();

            await _bookmarkRepository.UpdateAsync(bookmark);
        }
    }
}
