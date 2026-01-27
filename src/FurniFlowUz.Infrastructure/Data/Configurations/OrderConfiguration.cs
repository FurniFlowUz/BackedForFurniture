using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.CategoryId)
            .IsRequired();

        builder.Property(o => o.DeadlineDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.ProgressPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.CustomerId);

        builder.HasIndex(o => o.Status);

        builder.HasIndex(o => o.DeadlineDate);

        builder.HasIndex(o => o.AssignedConstructorId);

        builder.HasIndex(o => o.AssignedProductionManagerId);

        // Soft delete query filter
        builder.HasQueryFilter(o => !o.IsDeleted);

        // Relationships
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Category)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Contract)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ContractId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.AssignedConstructor)
            .WithMany(u => u.AssignedOrdersAsConstructor)
            .HasForeignKey(o => o.AssignedConstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.AssignedProductionManager)
            .WithMany(u => u.AssignedOrdersAsProductionManager)
            .HasForeignKey(o => o.AssignedProductionManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.FurnitureTypes)
            .WithOne(ft => ft.Order)
            .HasForeignKey(ft => ft.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.WorkTasks)
            .WithOne(wt => wt.Order)
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
