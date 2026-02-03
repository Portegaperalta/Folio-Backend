using Folio.Core.Domain.Entities;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Folio.Infrastructure.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FolderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Folder>> GetAllAsync(Guid userId)
        {
            return await _dbContext.Folders
                                   .Where(f => f.UserId == userId)
                                   .Include(f => f.Bookmarks)
                                   .ToListAsync();
        }

        public async Task<Folder?> GetByIdAsync(Guid userId, Guid folderId)
        {
            return await _dbContext.Folders
                                   .Where(f => f.Id == folderId)
                                   .Include(f => f.Bookmarks)
                                   .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Folder folderEntity)
        {
            _dbContext.Folders.Add(folderEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Folder folderEntity)
        {
            _dbContext.Folders.Update(folderEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Folder folderEntity)
        {
            _dbContext.Folders.Remove(folderEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountByUserAsync(Guid userId)
        {
            var folderCount = await _dbContext.Folders
                                        .Where(f => f.UserId == userId)
                                        .CountAsync();

            return folderCount;
        }
    }
}

