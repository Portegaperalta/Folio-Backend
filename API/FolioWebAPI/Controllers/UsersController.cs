using Folio.Core.Application.DTOs.User;
using Folio.Core.Interfaces;
using FolioWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FolioWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("Authenticated")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPut("update")]
        public async Task<ActionResult> Update([FromForm] UserUpdateDTO userUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization Failed");

            await _userService.UpdateUserAsync(currentUser.Id, userUpdateDTO);

            return NoContent();
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> Delete()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization Failed");

            await _userService.DeleteUserAsync(currentUser.Id);

            return NoContent();
        }
    }
}
