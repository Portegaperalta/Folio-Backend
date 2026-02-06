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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenGenerator tokenGenerator,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _currentUserService = currentUserService;
        }

        // POST: api/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponseDTO>>
            Register([FromForm] RegisterCredentialsDTO registerCredentialsDTO)
        {
            var user = new ApplicationUser
            {
                Name = registerCredentialsDTO.Name,
                UserName = registerCredentialsDTO.Email,
                Email = registerCredentialsDTO.Email,
                CreationDate = DateTime.UtcNow.Date,
                PhoneNumber = registerCredentialsDTO.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerCredentialsDTO.Password);

            if (result.Succeeded is not true)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem();
            }

            var applicationUser = await _userManager.FindByEmailAsync(user.Email);

            var userEntity = UserMapper.ToDomainEntity(applicationUser!);

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login([FromForm] LoginCredentialsDTO loginCredentialsDTO)
        {
            var applicationUser = await _userManager.FindByNameAsync(loginCredentialsDTO.Email);

            if (applicationUser is null)
            {
                return BadRequest("Authentication failed");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(applicationUser, loginCredentialsDTO.Password, false);

            if (result.Succeeded is not true)
            {
                return BadRequest("Authentication failed");
            }

            var userEntity = UserMapper.ToDomainEntity(applicationUser);

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token };
        }

        [HttpGet("renew-token")]
        [Authorize]
        public async Task<ActionResult<AuthenticationResponseDTO>> RenewToken()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null)
                return Unauthorized("Authentication failed");

            var token = _tokenGenerator.GenerateJwt(currentUser);

            return Ok(new AuthenticationResponseDTO { Token = token });
        }
    }
}