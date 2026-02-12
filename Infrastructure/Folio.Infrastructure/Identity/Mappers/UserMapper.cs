using Folio.Core.Domain.Entities;

namespace Folio.Infrastructure.Identity.Mappers
{
    public class UserMapper
    {
        public static User ToDomainEntity(ApplicationUser applicationUser)
        {
            var user = new User
                (applicationUser.Name,
                applicationUser.Email!,
                applicationUser.PasswordHash!,
                applicationUser.PhoneNumber);

            typeof(User).GetProperty("Id")!
                        .SetValue(user, applicationUser.Id);

            return user;
        }

        public static ApplicationUser ToApplicationUser(User domainUser)
        {
            return new ApplicationUser
            {
                Id = domainUser.Id,
                Name = domainUser.Name,
                Email = domainUser.Email,
                UserName = domainUser.Email,
                PhoneNumber = domainUser.PhoneNumber,
                IsDeleted = domainUser.IsDeleted,
                CreationDate = domainUser.CreationDate,
            };
        }

        public static void ToExistingApplicationUser(User userEntity, ApplicationUser applicationUser)
        {
            applicationUser.Name = userEntity.Name;
            applicationUser.Email = userEntity.Email;
            applicationUser.PhoneNumber = userEntity.PhoneNumber;
        }

        public static void UpdateFromDomain(ApplicationUser applicationUser, User domainUser)
        {
            applicationUser.Name = domainUser.Name;
            applicationUser.Email = domainUser.Email;
            applicationUser.PhoneNumber = domainUser.PhoneNumber;
            applicationUser.IsDeleted = domainUser.IsDeleted;
        }
    }
}
