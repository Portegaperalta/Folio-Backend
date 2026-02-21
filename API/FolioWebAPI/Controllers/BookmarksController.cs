using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.DTOs.Pagination;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/{folderId:guid}/bookmarks")]
    [Authorize]
    [EnableRateLimiting("Authenticated")]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICurrentUserService _currentUserService;

        public BookmarksController(IBookmarkService bookmarkService, ICurrentUserService currentUserService)
        {
            _bookmarkService = bookmarkService;
            _currentUserService = currentUserService;
        }

        // GET
        [HttpGet]
        [HttpGet("~/api/bookmarks")]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>>
        GetAll([FromRoute] Guid? folderId, [FromQuery] PaginationDTO paginationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            if (folderId.HasValue)
            {
                var total = await _bookmarkService.CountBookmarksByFolderIdAsync(currentUser.Id, folderId.Value);
                HttpContext.Response.Headers.Append("total-records-amount", total.ToString());
            }
            else
            {
                var total = await _bookmarkService.CountBookmarksByUserIdAsync(currentUser.Id); // add this method
                HttpContext.Response.Headers.Append("total-records-amount", total.ToString());
            }

            var bookmarksDTOs = await _bookmarkService.GetAllBookmarksAsync(currentUser.Id, folderId, paginationDTO);

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

        [HttpGet("count")]
        public async Task<ActionResult<int>> Count([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var bookmarkCount = await _bookmarkService.CountBookmarksByFolderIdAsync(currentUser.Id, folderId);

            return Ok(bookmarkCount);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult>
            Create([FromRoute] Guid folderId, [FromBody] BookmarkCreationDTO bookmarkCreationDTO)
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
            ([FromRoute] Guid bookmarkId, [FromRoute] Guid folderId, [FromBody] BookmarkUpdateDTO bookmarkUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            if (bookmarkId != bookmarkUpdateDTO.Id)
                return BadRequest("Bookmark ids must match");

            await _bookmarkService.UpdateBookmarkAsync(currentUser.Id, folderId, bookmarkUpdateDTO);

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