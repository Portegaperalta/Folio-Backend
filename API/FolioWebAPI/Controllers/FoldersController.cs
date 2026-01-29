using Folio.Core.Application.Services;
using FolioWebAPI.DTOs.Folder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FolioWebAPI.Controllers
{
    [ApiController]
    [Route("api/folders")]
    public class FoldersController : ControllerBase
    {
        private readonly FolderService folderService;

        public FoldersController(FolderService folderService)
        {
            this.folderService = folderService;
        }
    }
}
