using Folio.Core.Application.Services;
using Folio.Core.Interfaces;
using FolioWebAPI.DTOs.Folder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/folders")]
    public class FoldersController : ControllerBase
    {
        private readonly FolderService _folderService;
        private readonly ICurrentUserService _currentUserService;

        public FoldersController(FolderService folderService, ICurrentUserService currentUserService)
        {
            _folderService = folderService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FolderDTO>>> GetAll()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
            {
                return Unauthorized("Invalid email or password");
            }

            var folders = await _folderService.GetAllUserFoldersAsync(currentUser.Id);

            return Ok(folders);
        }
    }
}
