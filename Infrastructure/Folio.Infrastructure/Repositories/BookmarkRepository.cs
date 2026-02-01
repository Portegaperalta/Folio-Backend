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

        public async Task<IEnumerable<Bookmark>> GetAllAsync(Guid userId, int folderId) 
        {
            return await _dbContext.Bookmarks
                                   .Include(b => b.Folder)
                                   .Where(b => 
                                   b.FolderId == folderId &&
                                   b.UserId == userId)
                                   .ToListAsync();
        }

        public async Task<Bookmark?> GetByIdAsync(Guid userId, int folderId,Guid bookmarkId) 
        {
            return await _dbContext.Bookmarks
                                   .Include(b => b.Folder)
                                   .FirstOrDefaultAsync(b =>
                                   b.Id == bookmarkId &&
                                   b.FolderId == folderId &&
                                   b.UserId == userId);
        }

        public async Task<Bookmark?> GetByIdAsNoTrackingAsync(Guid userId, int folderId, Guid bookmarkId)
        {
            var bookmarkAsNoTracking = await _dbContext.Bookmarks
                                                       .Include(b => b.Folder)
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync(b =>
                                                       b.Id == bookmarkId &&
                                                       b.FolderId == folderId &&
                                                       b.UserId == userId);

            if (bookmarkAsNoTracking is null)
            {
                return null;
            }

            return bookmarkAsNoTracking;
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

        public async Task<bool> ExistsAsync(Guid userId, Guid bookmarkId)
        {
            var bookmark = await _dbContext.Bookmarks
                            .Where(b =>
                            b.Id == bookmarkId &&
                            b.UserId == userId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            if (bookmark is null)
            {
                return false;
            }

            return true;
        }

        public async Task<int> CountByFolderAsync(Guid userId, int folderId)
        {
            var bookmarkCount = await _dbContext.Bookmarks
                                                .Where(b => 
                                                b.FolderId == folderId &&
                                                b.UserId == userId)
                                                .CountAsync();

            return bookmarkCount;
        }
    }
}
