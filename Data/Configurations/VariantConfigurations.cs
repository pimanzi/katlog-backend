using katlog_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace katlog_backend.Data.Configurations;

public class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants", t =>
        {
            t.HasCheckConstraint(
                "CK_Variants_Status",
                "\"Status\" IN ('Active','Discontinued')"
            );
        });

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.VariantCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Colour)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Size)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Material)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Barcode)
            .HasMaxLength(50);

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(v => v.UpdatedAt)
            .HasDefaultValueSql("NOW()");
        
        builder.HasIndex(v => new { v.ProductId, v.VariantCode })
            .IsUnique();
        
        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}