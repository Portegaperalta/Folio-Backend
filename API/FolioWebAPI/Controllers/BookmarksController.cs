using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
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

        public BookmarksController(BookmarkService bookmarkService, ICurrentUserService currentUserService)
        {
            _bookmarkService = bookmarkService;
            _currentUserService = currentUserService;
        }

        // GET
        [HttpGet]
        [HttpGet("~/api/bookmarks")]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>> GetAll([FromRoute] Guid? folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmarksDTOs = await _bookmarkService.GetAllBookmarksAsync(currentUser.Id, folderId);

            return Ok(bookmarksDTOs);
        }

        [HttpGet("{bookmarkId:guid}", Name = "GetUserBookmark")]
        public async Task<ActionResult<BookmarkDTO>> GetById
            ([FromRoute] Guid folderId, [FromRoute] Guid bookmarkId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmarkDTO = await _bookmarkService.GetBookmarkByIdAsync(currentUser.Id, folderId, bookmarkId);

            if (bookmarkDTO is null)
                return NotFound($"Bookmark with id: {bookmarkId} not found");

            return Ok(bookmarkDTO);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> 
            Create([FromRoute] Guid folderId, [FromForm] BookmarkCreationDTO bookmarkCreationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var CreatedBookmarkDTO = await _bookmarkService.CreateBookmarkAsync(currentUser.Id, folderId, bookmarkCreationDTO);

            if (CreatedBookmarkDTO is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Something went wrong while creating bookmark" });
            }

            return CreatedAtRoute("GetUserBookmark", 
                new { folderId = folderId, bookmarkId = CreatedBookmarkDTO.Id }, CreatedBookmarkDTO);
        }

        // PUT
        [HttpPut("{bookmarkId:guid}")]
        public async Task<ActionResult> Update
            ([FromRoute] Guid bookmarkId,[FromRoute] Guid folderId, [FromForm] BookmarkUpdateDTO bookmarkUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            if (bookmarkId != bookmarkUpdateDTO.Id)
                return BadRequest("Bookmark ids must match");

            var bookmark = await _bookmarkService.GetBookmarkByIdAsync(currentUser.Id, folderId, bookmarkId);

            if (bookmark is null)
                return NotFound($"Bookmark with id: {bookmarkId} not found");

            if (bookmarkUpdateDTO.Name is not null)
            {
                await _bookmarkService.
                    ChangeBookmarkNameAsync(currentUser.Id, folderId, bookmark.Id, bookmarkUpdateDTO.Name);
            }

            if (bookmarkUpdateDTO.Url is not null)
            {
                await _bookmarkService.
                    ChangeBookmarkUrlAsync(currentUser.Id, folderId, bookmark.Id, bookmarkUpdateDTO.Url);
            }

            if (bookmarkUpdateDTO.IsMarkedFavorite.HasValue)
            {
                if (bookmarkUpdateDTO.IsMarkedFavorite is true)
                {
                    await _bookmarkService.MarkBookmarkAsFavoriteAsync(currentUser.Id, folderId, bookmark.Id);
                } else
                {
                    await _bookmarkService.UnmarkBookmarkAsFavoriteAsync(currentUser.Id, folderId, bookmark.Id);
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

            await _bookmarkService.MarkBookmarkAsVisitedAsync(currentUser.Id, folderId, bookmarkId);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{bookmarkId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid bookmarkId, [FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var isDeleted = await _bookmarkService.DeleteBookmarkAsync(currentUser.Id, folderId, bookmarkId);

            if (isDeleted is not true)
            {
                return NotFound($"Bookmark with id: {bookmarkId} not found");
            }

            return NoContent();
        }
    }
}