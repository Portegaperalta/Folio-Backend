using Folio.Core.Application.DTOs.User;
using Folio.Core.Application.Services;
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
        private readonly UserService _userService;
        private readonly CurrentUserService _currentUserService;

        public UsersController(UserService userService, CurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPut("updateProfile")]
        public async Task<ActionResult> Update([FromForm] UserUpdateDTO userUpdateDTO)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization Failed");

            await _userService.UpdateUserAsync(currentUser.Id, userUpdateDTO);

            return NoContent();
        }

        [HttpDelete("deleteAccount")]
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
