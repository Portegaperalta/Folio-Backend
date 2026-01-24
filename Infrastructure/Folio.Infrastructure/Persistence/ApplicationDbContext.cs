using Folio.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Folio.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        DbSet<Folder> Folders { get; set; }
        DbSet<Bookmark> Bookmarks { get; set; }
    }
}
