using Folio.Core.Domain.Entities;
using Folio.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Folio.Infrastructure.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> RegisterAsync(string name, string email, string password)
        {
            var applicationUser = new ApplicationUser
            {
              Name = name,
              Email = email,
              UserName = email,
              CreationDate = DateTime.UtcNow,
              IsDeleted = false
            };

            var result = _userManager.CreateAsync(applicationUser, password);

            if (result.IsCompletedSuccessfully is not true)
            {
                throw new InvalidOperationException(result.Exception!.Message);
            }

            return UserMapper.ToDomainEntity(applicationUser);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser is null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var validPassword = await _userManager.CheckPasswordAsync(applicationUser, password);

            if (validPassword is false)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var userEntity = UserMapper.ToDomainEntity(applicationUser);

            return userEntity;
        }
    }
}
