using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Domain.Entities;
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

        public async Task<AuthenticationResponseDTO> RegisterAsync(string name, string email, string password)
        {
            var newApplicationUser = new ApplicationUser
            {
                Name = name,
                Email = email,
                UserName = email,
                CreationDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(newApplicationUser, password);

            if (result.Succeeded is not true)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
            }

            var applicationUser = await _userManager.FindByEmailAsync(email);

            var userEntity = UserMapper.ToDomainEntity(applicationUser!);

            var token = _tokenGenerator.GenerateJwt(userEntity);

            return new AuthenticationResponseDTO { Token = token};
        }

        public async Task<AuthenticationResponseDTO?> LoginAsync(string email, string password)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var validPassword = await _userManager.CheckPasswordAsync(applicationUser, password);

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
