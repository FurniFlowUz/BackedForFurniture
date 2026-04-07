using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class OrderCategoryConfiguration : IEntityTypeConfiguration<OrderCategory>
{
    public void Configure(EntityTypeBuilder<OrderCategory> builder)
    {
        builder.HasKey(oc => oc.Id);

        builder.Property(oc => oc.OrderId)
            .IsRequired();

        builder.Property(oc => oc.CategoryId)
            .IsRequired();

        builder.Property(oc => oc.CreatedAt)
            .IsRequired();

        // Unique constraint - same category can't be added twice to same order
        builder.HasIndex(oc => new { oc.OrderId, oc.CategoryId })
            .IsUnique();

        // Indexes for faster queries
        builder.HasIndex(oc => oc.OrderId);
        builder.HasIndex(oc => oc.CategoryId);

        // Relationships
        builder.HasOne(oc => oc.Order)
            .WithMany(o => o.OrderCategories)
            .HasForeignKey(oc => oc.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oc => oc.Category)
            .WithMany(c => c.OrderCategories)
            .HasForeignKey(oc => oc.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
