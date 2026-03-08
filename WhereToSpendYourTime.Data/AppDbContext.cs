using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Data;

/// <summary>
/// Application database context integrating ASP.NET Core Identity
/// with domain entities such as items, reviews, comments, tags, and media.
/// 
/// Configures relationships, constraints, composite keys,
/// and cascade delete behaviors
/// </summary>
public class AppDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>,
        IdentityUserToken<string>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Categories available in the system</summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>Items that can be reviewed and tagged</summary>
    public DbSet<Item> Items { get; set; }

    /// <summary>User reviews for items</summary>
    public DbSet<Review> Reviews { get; set; }

    /// <summary>Comments attached to reviews</summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>Tags used to classify items</summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>Many-to-many join entity between items and tags</summary>
    public DbSet<ItemTag> ItemTags { get; set; }

    /// <summary>Media files associated with items</summary>
    public DbSet<Media> Media { get; set; }

    /// <summary>
    /// Configures entity relationships, composite keys, indexes,
    /// and delete behaviors
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });

        builder.Entity<ApplicationUser>()
            .Property(u => u.DisplayName)
            .IsRequired();

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.DisplayName)
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        builder.Entity<ItemTag>()
            .HasKey(it => new { it.ItemId, it.TagId });

        builder.Entity<ItemTag>()
            .HasOne(it => it.Item)
            .WithMany(i => i.ItemTags)
            .HasForeignKey(it => it.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ItemTag>()
            .HasOne(it => it.Tag)
            .WithMany(t => t.ItemTags)
            .HasForeignKey(it => it.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Media>()
            .HasOne(m => m.Item)
            .WithMany(i => i.Media)
            .HasForeignKey(m => m.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}