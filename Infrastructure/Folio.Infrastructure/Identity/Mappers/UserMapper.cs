using Folio.Core.Domain.Entities;

namespace Folio.Infrastructure.Identity.Mappers
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
