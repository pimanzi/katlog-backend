using Katlog.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Katlog.Api.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", t =>
        {
            t.HasCheckConstraint(
                "CK_Assets_AssetType",
                "\"AssetType\" IN ('MainImage','VariantImage','LifestyleImage','MarketingBanner','SizeGuide','TechnicalDocument')"
            );
            t.HasCheckConstraint(
                "CK_Assets_Status",
                "\"Status\" IN ('Uploaded','PendingReview','Approved','Rejected','Archived')"
            );
        });

        builder.HasKey(a => a.Id);

        builder.Property(a => a.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Title)
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.UploadedBy)
            .IsRequired();

        builder.Property(a => a.AssetType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasOne(a => a.Product)
            .WithMany(p => p.Assets)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(a => a.Variant)
            .WithMany(v => v.Assets)
            .HasForeignKey(a => a.VariantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}