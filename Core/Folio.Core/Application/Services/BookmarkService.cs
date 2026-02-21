using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.Mappers;
using Folio.Core.Domain.Exceptions.Folder;
using Folio.Core.Domain.Exceptions.Bookmark;
using Folio.Core.Interfaces;
using Folio.Core.Application.DTOs.Pagination;

namespace Folio.Core.Application.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly BookmarkMapper _bookmarkMapper;
        private readonly ICacheService _cacheService;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(5);

        public BookmarkService(IBookmarkRepository bookmarkRepository, BookmarkMapper bookmarkMapper, ICacheService cacheService)
        {
            _bookmarkRepository = bookmarkRepository;
            _bookmarkMapper = bookmarkMapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<BookmarkDTO>> GetAllBookmarksAsync(Guid userId, Guid? folderId, PaginationDTO paginationDTO)
        {
            var versionKey = $"folio:bookmarks:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;

            var cacheKey = folderId is null
                ? $"folio:bookmarks:{userId}:all:p{paginationDTO.Page}:r{paginationDTO.RecordsPerPage}:v{version}"
                : $"folio:bookmarks:{userId}:folder:{folderId}:all:p{paginationDTO.Page}:r{paginationDTO.RecordsPerPage}:v{version}";

            var cached = await _cacheService.GetAsync<List<BookmarkDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            IEnumerable<BookmarkDTO> bookmarksDTO;

            if (folderId is null)
            {
                var bookmarksByUserId = await _bookmarkRepository.GetAllByUserIdAsync(userId, paginationDTO);
                bookmarksDTO = bookmarksByUserId.Select(b => _bookmarkMapper.ToDto(b));
            }
            else
            {
                var bookmarks = await _bookmarkRepository.GetAllByUserAndFolderIdAsync(userId, folderId.Value, paginationDTO);
                bookmarksDTO = bookmarks.Select(b => _bookmarkMapper.ToDto(b));
            }

            var bookmarksDTOList = bookmarksDTO.ToList();

            await _cacheService.SetAsync(cacheKey, bookmarksDTOList, cacheDuration);

            return bookmarksDTOList;
        }

        public async Task<BookmarkDTO?> GetBookmarkByIdAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var versionKey = $"folio:bookmarks:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;

            var cacheKey = $"folio:bookmarks:{userId}:folder:{folderId}:byid:{bookmarkId}:v{version}";
            var cached = await _cacheService.GetAsync<BookmarkDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                return null;

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmark);

            await _cacheService.SetAsync(cacheKey, bookmarkDTO, cacheDuration);

            return bookmarkDTO;
        }

        public async Task<int> CountBookmarksByFolderIdAsync(Guid userId, Guid folderId)
        {
            var versionKey = $"folio:bookmarks:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;

            var cacheKey = $"folio:bookmarks:{userId}:folder:{folderId}:count:v{version}";
            var cached = await _cacheService.GetAsync<int?>(cacheKey);

            if (cached.HasValue)
                return cached.Value;

            var bookmarkCount = await _bookmarkRepository.CountByFolderAsync(userId, folderId);
            await _cacheService.SetAsync(cacheKey, bookmarkCount, cacheDuration);

            return bookmarkCount;
        }

        public async Task<BookmarkDTO> CreateBookmarkAsync(Guid userId, Guid folderId, BookmarkCreationDTO bookmarkCreationDTO)
        {
            ArgumentNullException.ThrowIfNull(bookmarkCreationDTO);

            var userFolder = await _bookmarkRepository.GetFolderByIdAsync(folderId, userId);

            if (userFolder is null)
                throw new FolderNotFoundException(folderId);

            var bookmarkEntity = _bookmarkMapper.ToEntity(userId, folderId, bookmarkCreationDTO);

            await _bookmarkRepository.AddAsync(bookmarkEntity);

            await _cacheService.IncrementAsync($"folio:bookmarks:{userId}:v");

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmarkEntity);

            return bookmarkDTO;
        }

        public async Task UpdateBookmarkAsync(Guid userId, Guid folderId, BookmarkUpdateDTO bookmarkUpdateDTO)
        {
            ArgumentNullException.ThrowIfNull(bookmarkUpdateDTO);

            var bookmarkEntity = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkUpdateDTO.Id);

            if (bookmarkEntity is null)
                throw new BookmarkNotFoundException(bookmarkUpdateDTO.Id);

            if (bookmarkUpdateDTO.Name is not null)
            {
                bookmarkEntity.ChangeName(bookmarkUpdateDTO.Name);
            }

            if (bookmarkUpdateDTO.Url is not null)
            {
                bookmarkEntity.ChangeUrl(bookmarkUpdateDTO.Url);
            }

            if (bookmarkUpdateDTO.IsMarkedFavorite.HasValue)
            {
                if (bookmarkUpdateDTO.IsMarkedFavorite is true)
                {
                    bookmarkEntity.MarkFavorite();
                }
                else
                {
                    bookmarkEntity.UnmarkFavorite();
                }
            }

            await _bookmarkRepository.UpdateAsync(bookmarkEntity);

            await _cacheService.IncrementAsync($"folio:bookmarks:{userId}:v");
        }

        public async Task<bool> DeleteBookmarkAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmarkEntity = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmarkEntity is null)
            {
                return false;
            }

            await _bookmarkRepository.DeleteAsync(bookmarkEntity);

            await _cacheService.IncrementAsync($"folio:bookmarks:{userId}:v");

            return true;
        }

        public async Task MarkBookmarkAsVisitedAsync(Guid userId, Guid folderId, Guid bookmarkId)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(userId, folderId, bookmarkId);

            if (bookmark is null)
                throw new BookmarkNotFoundException(bookmarkId);

            if (bookmark.UserId != userId)
                throw new BookmarkNotFoundException(bookmarkId);

            bookmark.Visit();

            await _bookmarkRepository.UpdateAsync(bookmark);

            await _cacheService.IncrementAsync($"folio:bookmarks:{userId}:v");
        }
    }
}
