using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class WarehouseTransactionConfiguration : IEntityTypeConfiguration<WarehouseTransaction>
{
    public void Configure(EntityTypeBuilder<WarehouseTransaction> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Type)
            .IsRequired();

        builder.Property(wt => wt.WarehouseItemId)
            .IsRequired();

        builder.Property(wt => wt.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(wt => wt.CreatedByUserId)
            .IsRequired();

        builder.Property(wt => wt.TransactionDate)
            .IsRequired();

        builder.Property(wt => wt.Notes)
            .HasMaxLength(1000);

        builder.Property(wt => wt.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(wt => wt.WarehouseItemId);

        builder.HasIndex(wt => wt.TeamId);

        builder.HasIndex(wt => wt.CreatedByUserId);

        builder.HasIndex(wt => wt.Type);

        builder.HasIndex(wt => wt.TransactionDate);

        // Soft delete query filter
        builder.HasQueryFilter(wt => !wt.IsDeleted);

        // Relationships
        builder.HasOne(wt => wt.WarehouseItem)
            .WithMany(wi => wi.Transactions)
            .HasForeignKey(wt => wt.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.Team)
            .WithMany(t => t.WarehouseTransactions)
            .HasForeignKey(wt => wt.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(wt => wt.CreatedByUser)
            .WithMany(u => u.WarehouseTransactions)
            .HasForeignKey(wt => wt.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.MaterialRequest)
            .WithOne(mr => mr.WarehouseTransaction)
            .HasForeignKey<MaterialRequest>(mr => mr.WarehouseTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
