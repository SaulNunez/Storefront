using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Storefront.Models.DAO;

namespace Storefront.Models;

public class StorefrontDbContext(DbContextOptions<StorefrontDbContext> options) : IdentityDbContext(options)
{
    public DbSet<AndroidRelease> AndroidReleases { get; set; }
    public DbSet<AndroidVariant> AndroidVariants { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<MacOsRelease> MacOsReleases { get; set; }
    public DbSet<MacOsVariant> MacOsVariants { get; set; }
    public DbSet<WindowsRelease> WindowsReleases { get; set; }
    public DbSet<WindowsVariant> WindowsVariants { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<AppCategories> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppCategories>().HasData([
            new AppCategories { Id = 1, Name = "Productivity" },
            new AppCategories { Id = 2, Name = "Utilities" },
            new AppCategories { Id = 3, Name = "Business" },
            new AppCategories{ Id = 4, Name = "Communication" },
            new AppCategories{ Id = 5, Name = "Education" },
            new AppCategories{ Id = 6, Name = "Finance" },
            new AppCategories{ Id = 7, Name = "Health & Fitness" },
            new AppCategories{ Id = 8, Name = "Lifestyle" },
            new AppCategories{ Id = 9, Name = "Entertainment" },
            new AppCategories{ Id = 10, Name = "Music & Audio" },
            new AppCategories{ Id = 11, Name = "Photo & Video" },
            new AppCategories{ Id = 12, Name = "News & Magazines" },
            new AppCategories{ Id = 13, Name = "Books & Reference" },
            new AppCategories{ Id = 14, Name = "Travel & Navigation" },
            new AppCategories{ Id = 15, Name = "Food & Drink" },
            new AppCategories{ Id = 16, Name = "Shopping" },
            new AppCategories{ Id = 17, Name = "Social" },
            new AppCategories{ Id = 18, Name = "Sports" },
            new AppCategories{ Id = 19, Name = "Weather" },
            new AppCategories{ Id = 20, Name = "Personalization" },
            new AppCategories{ Id = 21, Name = "Tools" },
            new AppCategories{ Id = 22, Name = "Maps & Navigation" }
        ]);
    }
}