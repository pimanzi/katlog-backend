using katlog_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace katlog_backend.Data;

public class KatlogDbContext : IdentityDbContext<AppUser>
{
    public KatlogDbContext(
        DbContextOptions<KatlogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Brand> Brands { get; set; }
    
    public DbSet <Asset> Assets { get; set; }
    
    public DbSet <Tag> Tags { get; set; }
    
    public DbSet <AssetTag> AssetTags { get; set; }
    public DbSet <Variant> Variants { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<AssetStatusHistory> AssetStatusHistories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {   
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<IdentityUserPasskey<string>>();
        modelBuilder.Ignore<IdentityPasskeyData>();

       
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KatlogDbContext).Assembly
        );
    }
}