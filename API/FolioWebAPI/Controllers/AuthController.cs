using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FolioWebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IAuthenticationService authenticationService, ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        // POST: api/register
        [HttpPost("register")]
        [EnableRateLimiting("Unauthenticated")]
        public async Task<ActionResult<AuthenticationResponseDTO>>
            Register([FromForm] RegisterCredentialsDTO registerCredentialsDTO)
        {

            var authenticationResponse = await _authenticationService.RegisterAsync(
                                               registerCredentialsDTO.Name,
                                               registerCredentialsDTO.Email,
                                               registerCredentialsDTO.Password,
                                               registerCredentialsDTO.PhoneNumber);

            return authenticationResponse;
        }

        [HttpPost("login")]
        [EnableRateLimiting("Unauthenticated")]
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
        [EnableRateLimiting("Authenticated")]
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