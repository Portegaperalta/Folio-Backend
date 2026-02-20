using Folio.Core.Application.DTOs.User;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FolioWebAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("Authenticated")]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UserAccountController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDetailsDTO>> GetProfileDetails()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authorization Failed");

            var userProfileDetails = await _userService.GetUserProfileDetails(currentUser.Id);

            return userProfileDetails;
        }

        [HttpPut("profile")]
        public async Task<ActionResult> Update([FromBody] UserUpdateDTO userUpdateDTO)
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
