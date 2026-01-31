using Folio.Core.Interfaces;
using Folio.Infrastructure.Identity;
using FolioWebAPI.DTOs.Auth;
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

        public UsersController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
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

            return new AuthenticationResponseDTO
            {
                Token = token,
            };
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

            return new AuthenticationResponseDTO
            { 
                Token = token
            };
        }
    }
}