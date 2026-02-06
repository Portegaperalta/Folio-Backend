using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Identity;
using Folio.Infrastructure.Identity.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FolioWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IAuthenticationService authenticationService, ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        // POST: api/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponseDTO>>
            Register([FromForm] RegisterCredentialsDTO registerCredentialsDTO)
        {

            var authenticationResponse = await _authenticationService.RegisterAsync(
                                               registerCredentialsDTO.Name,
                                               registerCredentialsDTO.Email,
                                               registerCredentialsDTO.Password);

            return authenticationResponse;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login([FromForm] LoginCredentialsDTO loginCredentialsDTO)
        {

            var authenticationResponseDTO = await _authenticationService
                                                 .LoginAsync(loginCredentialsDTO.Email, loginCredentialsDTO.Password);

            if (authenticationResponseDTO is null)
            {
                return Unauthorized("Authentication failed");
            }

            return authenticationResponseDTO;
        }

        [HttpGet("renew-token")]
        [Authorize]
        public async Task<ActionResult<AuthenticationResponseDTO>> RenewToken()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authentication failed");

            var authenticationResponseDTO = _authenticationService.RenewToken(currentUser);

            return Ok(authenticationResponseDTO);
        }
    }
}