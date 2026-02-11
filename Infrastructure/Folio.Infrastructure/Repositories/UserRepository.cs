using Folio.Core.Domain.Entities;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Identity.Mappers;
using Folio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Folio.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            var applicationUser = await _dbContext.Users
                                                  .Where(u => u.Id == userId)
                                                  .FirstOrDefaultAsync();

            return applicationUser != null
                ? UserMapper.ToDomainEntity(applicationUser)
                : null;
        }

        public async Task UpdateUserAsync(User userEntity)
        {
            var applicationUser = UserMapper.ToApplicationUser(userEntity);

            _dbContext.Users.Update(applicationUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User userEntity)
        {
            var applicationUser = UserMapper.ToApplicationUser(userEntity);

            _dbContext.Users.Remove(applicationUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}
