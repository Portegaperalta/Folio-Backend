using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.Mappers;
using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/folders")]
    [Authorize]
    public class FoldersController : ControllerBase
    {
        private readonly FolderService _folderService;
        private readonly ICurrentUserService _currentUserService;
        private readonly FolderMapper _folderMapper;

        public FoldersController(FolderService folderService,
            ICurrentUserService currentUserService, 
            FolderMapper folderMapper)
        {
            _folderService = folderService;
            _currentUserService = currentUserService;
            _folderMapper = folderMapper;
        }

        //GET
        [HttpGet(Name = "GetAllUserFolders")]
        public async Task<ActionResult<IEnumerable<FolderDTO>>> GetAll()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var foldersDTOs = await _folderService.GetAllUserFoldersAsync(currentUser.Id);

            return Ok(foldersDTOs);
        }

        [HttpGet("{folderId:guid}", Name = "GetUserFolder")]
        public async Task<ActionResult<FolderDTO?>> GetById([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var folderDTO = await _folderService.GetUserFolderByIdAsync(currentUser.Id, folderId);

            if (folderDTO is null)
                return NotFound($"Folder with Id:{folderId} not found");

            return Ok(folderDTO);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            int folderCount = await _folderService.CountUserFolders(currentUser.Id);

            return folderCount;
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] FolderCreationDTO folderCreationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var folderEntity = _folderMapper.ToEntity(currentUser.Id, folderCreationDTO);

            await _folderService.CreateUserFolder(folderEntity);

            var folderDTO = _folderMapper.ToDto(folderEntity);

            return CreatedAtRoute("GetUserFolder", new {folderId = folderDTO.Id}, folderDTO);
        }

        // PUT
        [HttpPut("{folderId:guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid folderId, [FromForm] FolderUpdateDTO folderUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization failed");

            var folder = await _folderService.GetFolderByIdAsync(currentUser.Id, folderId);

            if (folder is null)
                return NotFound($"Folder with id {folderId} not found");

            if (folderUpdateDTO.Name is not null)
            {
               await _folderService.ChangeFolderNameAsync(currentUser.Id, folderId, folderUpdateDTO.Name);
            }

            if (folderUpdateDTO.IsMarkedFavorite.HasValue)
            {
                if (folderUpdateDTO.IsMarkedFavorite is true)
                {
                    await _folderService.MarkFolderAsFavoriteAsync(currentUser.Id, folderId);
                } else
                {
                    await _folderService.UnmarkFolderAsFavoriteAsync(currentUser.Id, folderId);
                }
            }

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

            var folder = await _folderService.GetFolderByIdAsync(currentUser.Id, folderId);

            if (folder is null)
                return NotFound($"Folder with id {folderId} not found");

            await _folderService.DeleteFolderAsync(currentUser.Id, folder);

            return NoContent();
        }
    }
}