using Folio.Core.Domain;
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
                        .HasDefaultValueSql("gen_random-uuid()");

            modelBuilder.Entity<Folder>()
                        .HasOne<ApplicationUser>()
                        .WithMany()
                        .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Bookmark>()
                        .HasOne<ApplicationUser>()
                        .WithMany()
                        .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
    }
}
