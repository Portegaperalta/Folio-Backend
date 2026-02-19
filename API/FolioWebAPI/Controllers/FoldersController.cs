using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/folders")]
    [Authorize]
    [EnableRateLimiting("Authenticated")]
    public class FoldersController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheKey = "get-folder";

        public FoldersController(IFolderService folderService,
            ICurrentUserService currentUserService, IOutputCacheStore outputCacheStore)
        {
            _folderService = folderService;
            _currentUserService = currentUserService;
            _outputCacheStore = outputCacheStore;
        }

        //GET
        [HttpGet(Name = "GetAllUserFolders")]
        [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<IEnumerable<FolderDTO>>> GetAll()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var foldersDTOs = await _folderService.GetAllFoldersDTOsAsync(currentUser.Id);

            return Ok(foldersDTOs);
        }

        [HttpGet("{folderId:guid}", Name = "GetUserFolder")]
        [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<FolderDTO?>> GetById([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var folderDTO = await _folderService.GetFolderDTOByIdAsync(currentUser.Id, folderId);

            if (folderDTO is null)
                return NotFound($"Folder with Id:{folderId} not found");

            return Ok(folderDTO);
        }

        [HttpGet("count")]
        [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<int>> Count()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            int folderCount = await _folderService.CountFoldersAsync(currentUser.Id);

            return folderCount;
        }

        // POST
        [HttpPost]
        public async Task<ActionResult<FolderDTO>> Create([FromBody] FolderCreationDTO folderCreationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            await _outputCacheStore.EvictByTagAsync(cacheKey, default);

            var CreatedFolderDTO = await _folderService.CreateFolderAsync(currentUser.Id, folderCreationDTO);

            if (CreatedFolderDTO is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Something went wrong while creating folder" });
            }

            return CreatedAtRoute("GetUserFolder", new { folderId = CreatedFolderDTO.Id }, CreatedFolderDTO);
        }

        // PUT
        [HttpPut("{folderId:guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid folderId, [FromBody] FolderUpdateDTO folderUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            await _outputCacheStore.EvictByTagAsync(cacheKey, default);

            await _folderService.UpdateFolderAsync(folderId, currentUser.Id, folderUpdateDTO);

            return NoContent();
        }

        [HttpPut("{folderId:guid}/visit")]
        public async Task<ActionResult> Visit([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            await _folderService.MarkFolderAsVisitedAsync(currentUser.Id, folderId);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{folderId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            await _outputCacheStore.EvictByTagAsync(cacheKey, default);

            var isDeleted = await _folderService.DeleteFolderAsync(currentUser.Id, folderId);

            if (isDeleted is not true)
            {
                return NotFound($"Folder with id {folderId} not found");
            }

            return NoContent();
        }
    }
}