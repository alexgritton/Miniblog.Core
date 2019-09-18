using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Miniblog.Core.Models;

namespace GenericBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<User> User { get; set; }
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(x => x.NormalizedEmail).HasMaxLength(190);
            builder.Entity<User>().Property(x => x.UserName).HasMaxLength(190);
            builder.Entity<User>().Property(x => x.Email).HasMaxLength(190);
            builder.Entity<User>().Property(x => x.NormalizedUserName).HasMaxLength(190);
            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(190);
            builder.Entity<IdentityRole>().Property(x => x.Name).HasMaxLength(190);
            builder.Entity<IdentityRole>().Property(x => x.NormalizedName).HasMaxLength(190);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(190);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(190);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(190);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(190);
        }
    }
}
