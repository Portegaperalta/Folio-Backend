using Folio.Core.Domain;
using Folio.Core.Interfaces;
using Folio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Folio.Infrastructure.Repositories
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookmarkRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Bookmark>> GetAllAsync(int folderId) 
        {
            return await _dbContext.Bookmarks
                                   .Where(b => b.FolderId == folderId)
                                   .ToListAsync();
        }

        public async Task<Bookmark?> GetByIdAsync(Guid bookmarkId) 
        {
            return await _dbContext.Bookmarks
                                   .Where(b => b.Id == bookmarkId)
                                   .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Bookmark bookmarkEntity)
        {
            _dbContext.Bookmarks.Add(bookmarkEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bookmark bookmarkEntity)
        {
            _dbContext.Bookmarks.Update(bookmarkEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Bookmark bookmarkEntity)
        {
            _dbContext.Bookmarks.Remove(bookmarkEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid bookmarkId)
        {
            var bookmark = await _dbContext.Bookmarks
                            .Where(b => b.Id == bookmarkId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            if (bookmark is null)
            {
                return false;
            }

            return true;
        }

        public async Task<int> CountByFolderAsync(int folderId)
        {
            var bookmarkCount = await _dbContext.Bookmarks
                                                .Where(b => b.FolderId == folderId)
                                                .CountAsync();

            return bookmarkCount;
        }
    }
}
