using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
using FolioWebAPI.DTOs.Bookmark;
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
    }
}