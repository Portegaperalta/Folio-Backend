using Folio.Core.Domain;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace FolioWebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext!
                                                 .User
                                                 .Claims
                                                 .Where(c => c.Type == "id")
                                                 .FirstOrDefault();

            if (userIdClaim is null)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            return int.Parse(userIdClaim.Value);
        }

        public string GetCurrentUserEmail()
        {
            var userEmailClaim = _httpContextAccessor.HttpContext!
                                                    .User
                                                    .Claims
                                                    .Where(c => c.Type == "email")
                                                    .FirstOrDefault();

            if (userEmailClaim is null)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            return userEmailClaim.Value;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userEmailClaim = GetCurrentUserEmail();

            if (userEmailClaim is null)
            {
                return null;
            }

            var applicationUser = await _userManager.FindByEmailAsync(GetCurrentUserEmail());

            if (applicationUser is null || applicationUser.IsDeleted is true)
            {
                return null;
            }

            var user = UserMapper.ToDomainEntity(applicationUser);

            return user;
        }
    }
}
