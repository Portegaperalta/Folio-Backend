using Folio.Core.Domain.Entities;
using Folio.Infrastructure.Identity;

namespace FolioWebAPI.Mappers
{
    public class UserMapper
    {
        public User ToEntity(ApplicationUser applicationUser)
        {
            return new User(applicationUser.Name,
                applicationUser.Email!,
                applicationUser.PasswordHash!,
                applicationUser.PhoneNumber);
        }
    }
}
