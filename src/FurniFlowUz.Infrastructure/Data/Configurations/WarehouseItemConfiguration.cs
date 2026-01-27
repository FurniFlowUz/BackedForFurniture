using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
{
    public void Configure(EntityTypeBuilder<WarehouseItem> builder)
    {
        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wi => wi.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wi => wi.CurrentStock)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(wi => wi.MinimumStock)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(wi => wi.Unit)
            .IsRequired();

        builder.Property(wi => wi.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(wi => wi.Description)
            .HasMaxLength(500);

        builder.Property(wi => wi.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(wi => wi.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(wi => wi.SKU)
            .IsUnique();

        builder.HasIndex(wi => wi.Name);

        builder.HasIndex(wi => wi.IsActive);

        // Soft delete query filter
        builder.HasQueryFilter(wi => !wi.IsDeleted);

        // Relationships
        builder.HasMany(wi => wi.Transactions)
            .WithOne(wt => wt.WarehouseItem)
            .HasForeignKey(wt => wt.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
