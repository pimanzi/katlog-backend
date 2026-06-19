using katlog_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace katlog_backend.Data.Configurations;

public class AssetStatusHistoryConfiguration : IEntityTypeConfiguration<AssetStatusHistory>
{
    public void Configure(EntityTypeBuilder<AssetStatusHistory> builder)
    {
        builder.ToTable("AssetStatusHistories", t =>
        {
            t.HasCheckConstraint(
                "CK_AssetStatusHistories_PreviousStatus",
                "\"PreviousStatus\" IN ('Uploaded','PendingReview','Approved','Rejected','Archived')"
            );
            t.HasCheckConstraint(
                "CK_AssetStatusHistories_NewStatus",
                "\"NewStatus\" IN ('Uploaded','PendingReview','Approved','Rejected','Archived')"
            );
        });

        builder.HasKey(h => h.Id);

        builder.Property(h => h.PreviousStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.ChangedBy)
            .IsRequired();

        builder.Property(h => h.Comment)
            .HasMaxLength(500);

        builder.HasOne(h => h.Asset)
            .WithMany(a => a.StatusHistory)
            .HasForeignKey(h => h.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}