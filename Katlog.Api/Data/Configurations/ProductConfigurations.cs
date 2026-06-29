using Katlog.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Katlog.Api.Data.Configurations;


public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", t =>
        {
            t.HasCheckConstraint(
                "CK_Products_Status",
                "\"Status\" IN ('Draft','InReview','ReadyToPublish', 'Published','Archived')"
            );
            t.HasCheckConstraint(
                "CK_Products_Season",
                "\"Season\" IN ('Spring','Summer','Autumn','Winter')"
            );
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ProductCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ProductCode)
            .IsUnique();

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        
        builder.Property(p => p.Season)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(p => p.TargetMarket)
            .HasColumnType("text[]");
        
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()");
        
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}