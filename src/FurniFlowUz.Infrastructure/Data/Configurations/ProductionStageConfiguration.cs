using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class ProductionStageConfiguration : IEntityTypeConfiguration<ProductionStage>
{
    public void Configure(EntityTypeBuilder<ProductionStage> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ps => ps.StageType)
            .IsRequired();

        builder.Property(ps => ps.SequenceOrder)
            .IsRequired();

        builder.Property(ps => ps.EstimatedDurationHours)
            .HasPrecision(10, 2);

        builder.Property(ps => ps.Description)
            .HasMaxLength(500);

        builder.Property(ps => ps.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ps => ps.StageType);

        builder.HasIndex(ps => ps.SequenceOrder);

        builder.HasIndex(ps => ps.IsActive);

        // Soft delete query filter
        builder.HasQueryFilter(ps => !ps.IsDeleted);

        // Relationships
        builder.HasMany(ps => ps.WorkTasks)
            .WithOne(wt => wt.ProductionStage)
            .HasForeignKey(wt => wt.ProductionStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
