using katlog_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace katlog_backend.Data;

public class KatlogDbContext : DbContext
{
    public KatlogDbContext(
        DbContextOptions<KatlogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KatlogDbContext).Assembly
        );
    }
}