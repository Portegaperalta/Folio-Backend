using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
using FolioWebAPI.DTOs.Folder;
using FolioWebAPI.Mappers;
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
            {
                return Unauthorized("Authorization failed");
            }

            var folders = await _folderService.GetAllUserFoldersAsync(currentUser.Id);

            return Ok(folders);
        }

        [HttpGet("{folderId:guid}", Name = "GetUserFolder")]
        public async Task<ActionResult<FolderDTO?>> GetById([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
            {
                return Unauthorized("Authorization failed");
            }

            var folder = await _folderService.GetUserFolderByIdAsync(currentUser.Id, folderId);

            if (folder is null)
            {
                return NotFound($"Folder with Id:{folderId} not found");
            }

            var folderDTO = _folderMapper.ToDto(folder);

            return Ok(folderDTO);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] FolderCreationDTO folderCreationDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
            {
                return Unauthorized("Authorization failed");
            }

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
            {
                return Unauthorized("Authorization failed");
            }

            var folder = await _folderService.GetUserFolderByIdAsync(currentUser.Id, folderId);

            if (folder is null)
            {
                return NotFound($"Folder with id {folderId} not found");
            }

            if (folderUpdateDTO.Name is not null)
            {
               await _folderService.ChangeUserFolderNameAsync(currentUser.Id, folderId, folderUpdateDTO.Name);
            }

            if (folderUpdateDTO.IsMarkedFavorite is not null)
            {
                await _folderService.MarkUserFolderAsFavoriteAync(currentUser.Id, folderId);
            }

            return NoContent();
        }

        // DELETE
        [HttpDelete("{folderId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid folderId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
            {
                return Unauthorized("Authorization failed");
            }

            var folder = await _folderService.GetUserFolderByIdAsync(currentUser.Id, folderId);

            if (folder is null)
            {
                return NotFound($"Folder with id {folderId} not found");
            }

            await _folderService.DeleteUserFolderAsync(currentUser.Id, folder);

            return NoContent();
        }
    }
}