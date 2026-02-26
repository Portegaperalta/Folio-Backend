using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions.User;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Identity.Mappers;
using Microsoft.AspNetCore.Identity;

namespace Folio.Infrastructure.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthenticationService(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthenticationResponseDTO> 
            RegisterAsync(RegisterCredentialsDTO registerCredentialsDTO)
        {
            var newApplicationUser = new ApplicationUser
            {
                Name = registerCredentialsDTO.Name,
                UserName = registerCredentialsDTO.Email,
                Email = registerCredentialsDTO.Email,
                PhoneNumber = registerCredentialsDTO.PhoneNumber,
                CreationDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(newApplicationUser, registerCredentialsDTO.Password);

            if (result.Succeeded is false)
            {
                var errorMessages = string.Join(",", result.Errors.Select(e => e.Description));
                throw new RegistrationFailedException();
            }

            var applicationUser = await _userManager.FindByEmailAsync(registerCredentialsDTO.Email);

            var userEntity = UserMapper.ToDomainEntity(applicationUser!);

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token};
        }

        public async Task<AuthenticationResponseDTO?> LoginAsync(LoginCredentialsDTO loginCredentialsDTO)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginCredentialsDTO.Email);

            if (applicationUser is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var validPassword = await _userManager.CheckPasswordAsync(applicationUser, loginCredentialsDTO.Password);

            if (validPassword is false)
                throw new UnauthorizedAccessException("Invalid credentials");

            var userEntity = UserMapper.ToDomainEntity(applicationUser);

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token };
        }

        public AuthenticationResponseDTO RenewToken(User userEntity)
        {
            if (userEntity is null)
                throw new ArgumentException("User entity cannot be null");

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token };
        }
    }
}
