using Folio.Core.Domain.Entities;
using Folio.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Folio.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                    .Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Folder>()
                        .HasOne<ApplicationUser>()
                        .WithMany()
                        .HasForeignKey(f => f.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bookmark>()
                        .HasOne<Folder>()
                        .WithMany()
                        .HasForeignKey(b => b.FolderId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Error> Errors { get; set; }
    }
}
