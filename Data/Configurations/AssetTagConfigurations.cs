using katlog_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace katlog_backend.Data.Configurations;

public class AssetTagConfiguration : IEntityTypeConfiguration<AssetTag>
{
    public void Configure(EntityTypeBuilder<AssetTag> builder)
    {
        builder.ToTable("AssetTags");
        
        builder.HasKey(at => new { at.AssetId, at.TagId });

        builder.HasOne(at => at.Asset)
            .WithMany(a => a.AssetTags)
            .HasForeignKey(at => at.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(at => at.Tag)
            .WithMany(t => t.AssetTags)
            .HasForeignKey(at => at.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}