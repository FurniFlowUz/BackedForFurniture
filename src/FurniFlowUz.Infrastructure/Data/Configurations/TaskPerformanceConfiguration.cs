using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class TaskPerformanceConfiguration : IEntityTypeConfiguration<TaskPerformance>
{
    public void Configure(EntityTypeBuilder<TaskPerformance> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.ActualDuration)
            .IsRequired();

        builder.Property(tp => tp.EfficiencyPercent)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(tp => tp.QualityScore)
            .IsRequired();

        builder.Property(tp => tp.CompletedAt)
            .IsRequired();

        builder.Property(tp => tp.ReworkReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(tp => tp.DetailTask)
            .WithOne(dt => dt.Performance)
            .HasForeignKey<TaskPerformance>(tp => tp.DetailTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tp => tp.DetailTaskId)
            .IsUnique();
    }
}
