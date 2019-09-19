using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Miniblog.Core.Models;

namespace GenericBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<PostCategory> PostCategory { get; set; }
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(x => x.Id).HasMaxLength(190);
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
            builder.Entity<PostCategory>().HasKey(x=> new {x.CategoryName, x.PostId});
            builder.Entity<PostCategory>().HasOne(x=>x.Category).WithMany(x=>x.PostCategories).HasForeignKey(x=>x.CategoryName);
            builder.Entity<PostCategory>().HasOne(x=>x.Post).WithMany(x=>x.PostCategories).HasForeignKey(x=>x.PostId);

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                foreach (var property in entity.ClrType.GetProperties())
                {
                    // if (property.PropertyType == typeof(List<string>))
                    // {
                    //     builder.Entity(entity.Name).Property(property.Name).HasConversion(new ValueConverter<List<string>, string>(v => v.ToJson(), v => v.FromJson<List<string>>())).HasColumnType("json");
                    // }
                    // else if (property.PropertyType == typeof(Dictionary<string, string>))
                    // {
                    //     builder.Entity(entity.Name).Property(property.Name).HasConversion(new ValueConverter<Dictionary<string, string>, string>(v => v.ToJson(), v => v.FromJson<Dictionary<string, string>>())).HasColumnType("json");
                    // }
                    // else if (property.PropertyType == typeof(List<List<string>>))
                    // {
                    //     builder.Entity(entity.Name).Property(property.Name).HasConversion(new ValueConverter<List<List<string>>, string>(v => v.ToJson(), v => v.FromJson<List<List<string>>>())).HasColumnType("json");
                    // }
                    if (property.PropertyType == typeof(bool))
                    {

                        builder.Entity(entity.Name).Property(property.Name).HasConversion(new BoolToZeroOneConverter<short>());
                    }
                }
            }
        }
    }
}
