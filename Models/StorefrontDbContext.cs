using Microsoft.EntityFrameworkCore;
using Storefront.Models.DAO;

namespace Storefront.Models;

public class StorefrontDbContext(DbContextOptions<StorefrontDbContext> options) : DbContext(options)
{
    public DbSet<AndroidRelease> AndroidReleases { get; set; }
    public DbSet<AndroidVariant> AndroidVariants { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<MacOsRelease> MacOsReleases { get; set; }
    public DbSet<MacOsVariant> MacOsVariants { get; set; }
    public DbSet<WindowsRelease> WindowsReleases { get; set; }
    public DbSet<WindowsApplicationVariant> WindowsVariants { get; set; }
}