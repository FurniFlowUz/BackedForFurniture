using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class MaterialRequestConfiguration : IEntityTypeConfiguration<MaterialRequest>
{
    public void Configure(EntityTypeBuilder<MaterialRequest> builder)
    {
        builder.HasKey(mr => mr.Id);

        builder.Property(mr => mr.WarehouseTransactionId)
            .IsRequired();

        builder.Property(mr => mr.TeamId)
            .IsRequired();

        builder.Property(mr => mr.RequestedByUserId)
            .IsRequired();

        builder.Property(mr => mr.ConfirmationStatus)
            .IsRequired();

        builder.Property(mr => mr.Notes)
            .HasMaxLength(1000);

        builder.Property(mr => mr.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(mr => mr.WarehouseTransactionId)
            .IsUnique();

        builder.HasIndex(mr => mr.TeamId);

        builder.HasIndex(mr => mr.RequestedByUserId);

        builder.HasIndex(mr => mr.ConfirmedByUserId);

        builder.HasIndex(mr => mr.ConfirmationStatus);

        // Soft delete query filter
        builder.HasQueryFilter(mr => !mr.IsDeleted);

        // Relationships
        builder.HasOne(mr => mr.WarehouseTransaction)
            .WithOne(wt => wt.MaterialRequest)
            .HasForeignKey<MaterialRequest>(mr => mr.WarehouseTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mr => mr.Team)
            .WithMany(t => t.MaterialRequests)
            .HasForeignKey(mr => mr.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.RequestedByUser)
            .WithMany(u => u.MaterialRequests)
            .HasForeignKey(mr => mr.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.ConfirmedByUser)
            .WithMany(u => u.ConfirmedMaterialRequests)
            .HasForeignKey(mr => mr.ConfirmedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
