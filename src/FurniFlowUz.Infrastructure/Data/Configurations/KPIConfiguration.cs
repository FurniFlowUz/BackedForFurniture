using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class KPIConfiguration : IEntityTypeConfiguration<KPI>
{
    public void Configure(EntityTypeBuilder<KPI> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Date)
            .IsRequired();

        builder.Property(k => k.OrdersCompleted)
            .IsRequired();

        builder.Property(k => k.Revenue)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(k => k.AverageCompletionTime)
            .HasPrecision(10, 2);

        builder.Property(k => k.TeamProductivity)
            .HasPrecision(5, 2);

        builder.Property(k => k.ActiveOrders)
            .IsRequired();

        builder.Property(k => k.DelayedOrders)
            .IsRequired();

        builder.Property(k => k.MaterialUtilizationRate)
            .HasPrecision(5, 2);

        builder.Property(k => k.CustomerSatisfactionRate)
            .HasPrecision(5, 2);

        builder.Property(k => k.NewCustomers)
            .IsRequired();

        builder.Property(k => k.Notes)
            .HasMaxLength(1000);

        builder.Property(k => k.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(k => k.Date)
            .IsUnique();

        // Note: KPI does not inherit from BaseAuditableEntity, so no soft delete filter
    }
}
