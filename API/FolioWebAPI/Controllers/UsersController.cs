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
                UserName = registerCredentialsDTO.Email,
                Email = registerCredentialsDTO.Email,
                PhoneNumber = registerCredentialsDTO.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user);

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
    }
}