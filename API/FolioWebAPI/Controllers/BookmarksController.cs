using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
using FolioWebAPI.DTOs.Bookmark;
using FolioWebAPI.DTOs.Folder;
using FolioWebAPI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/{folderId:guid}/bookmarks")]
    [Authorize]
    public class BookmarksController : ControllerBase
    {
        private readonly BookmarkService _bookmarkService;
        private readonly ICurrentUserService _currentUserService;
        private readonly BookmarkMapper _bookmarkMapper;

        public BookmarksController(BookmarkService bookmarkService, ICurrentUserService currentUserService, BookmarkMapper bookmarkMapper)
        {
            _bookmarkService = bookmarkService;
            _currentUserService = currentUserService;
            _bookmarkMapper = bookmarkMapper;
        }

        // GET
        [HttpGet]
        [HttpGet("~/api/bookmarks")]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>> GetAll([FromRoute] Guid? folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmarks = await _bookmarkService.GetAllUserBookmarksAsync(currentUser.Id, folderId);

            var bookmarksDTOs = bookmarks.Select(b => _bookmarkMapper.ToDto(b));

            return Ok(bookmarksDTOs);
        }

        [HttpGet("{bookmarkId:guid}", Name = "GetUserBookmark")]
        public async Task<ActionResult<BookmarkDTO>> GetById([FromRoute] Guid folderId, [FromRoute] Guid bookmarkId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmark = await _bookmarkService.GetUserBookmarkByIdAsync(currentUser.Id, folderId, bookmarkId);

            if (bookmark is null)
                return NotFound($"Bookmark with id: {bookmarkId} not found");

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmark);

            return Ok(bookmarkDTO);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] BookmarkCreationDTO bookmarkCreationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmarkEntity = _bookmarkMapper.ToEntity(currentUser.Id, bookmarkCreationDTO);

            await _bookmarkService.CreateUserBookmarkAsync(bookmarkEntity);

            var bookmarkDTO = _bookmarkMapper.ToDto(bookmarkEntity);

            return CreatedAtRoute("GetUserBookmark", new { bookmarkId = bookmarkEntity.Id }, bookmarkDTO);
        }

        // PUT
        [HttpPut("{bookmarkId:guid}")]
        public async Task<ActionResult> Update
            ([FromRoute] Guid bookmarkId,[FromRoute] Guid folderId, [FromForm] BookmarkUpdateDTO bookmarkUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmark = await _bookmarkService.GetUserBookmarkByIdAsync(currentUser.Id, folderId, bookmarkUpdateDTO.Id);

            if (bookmark is null)
                return NotFound($"Bookmark with id: {bookmarkUpdateDTO.Id} not found");

            if (bookmarkUpdateDTO.Name is not null)
            {
                await _bookmarkService.
                    ChangeUserBookmarkNameAsync(currentUser.Id, folderId, bookmark.Id, bookmarkUpdateDTO.Name);
            }

            if (bookmarkUpdateDTO.Url is not null)
            {
                await _bookmarkService.
                    ChangeUserBookmarkUrlAsync(currentUser.Id, folderId, bookmark.Id, bookmarkUpdateDTO.Url);
            }

            if (bookmarkUpdateDTO.IsMarkedFavorite.HasValue)
            {
                if (bookmarkUpdateDTO.IsMarkedFavorite is true)
                {
                    await _bookmarkService.MarkUserBookmarkAsFavoriteAsync(currentUser.Id, folderId, bookmark.Id);
                } else
                {
                    await _bookmarkService.UnmarkUserBookmarkAsFavoriteAsync(currentUser.Id, folderId, bookmark.Id);
                }
            }

            return NoContent();
        }

        [HttpPut("{bookmarkId:guid}/visit")]
        public async Task<ActionResult> Visit([FromRoute] Guid bookmarkId, [FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            await _bookmarkService.MarkUserBookmarkAsVisitedAsync(currentUser.Id, folderId, bookmarkId);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{bookmarkId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid bookmarkId, [FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmark = await _bookmarkService.GetUserBookmarkByIdAsync(currentUser.Id, folderId, bookmarkId);

            if (bookmark is null)
                return NotFound($"Bookmark with id: {bookmarkId} not found");

            await _bookmarkService.DeleteUserBookmarkAsync(currentUser.Id, folderId, bookmarkId);

            return NoContent();
        }
    }
}