using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class FurnitureTypeConfiguration : IEntityTypeConfiguration<FurnitureType>
{
    public void Configure(EntityTypeBuilder<FurnitureType> builder)
    {
        builder.HasKey(ft => ft.Id);

        builder.Property(ft => ft.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ft => ft.OrderId)
            .IsRequired();

        builder.Property(ft => ft.ProgressPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(ft => ft.Notes)
            .HasMaxLength(1000);

        builder.Property(ft => ft.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ft => ft.OrderId);

        builder.HasIndex(ft => ft.TechnicalSpecificationId);

        // Soft delete query filter
        builder.HasQueryFilter(ft => !ft.IsDeleted);

        // Relationships
        builder.HasOne(ft => ft.Order)
            .WithMany(o => o.FurnitureTypes)
            .HasForeignKey(ft => ft.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ft => ft.TechnicalSpecification)
            .WithOne(ts => ts.FurnitureType)
            .HasForeignKey<FurnitureType>(ft => ft.TechnicalSpecificationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ft => ft.Details)
            .WithOne(d => d.FurnitureType)
            .HasForeignKey(d => d.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ft => ft.Drawings)
            .WithOne(d => d.FurnitureType)
            .HasForeignKey(d => d.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ft => ft.WorkTasks)
            .WithOne(wt => wt.FurnitureType)
            .HasForeignKey(wt => wt.FurnitureTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
