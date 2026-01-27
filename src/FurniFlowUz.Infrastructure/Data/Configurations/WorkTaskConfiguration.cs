using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class WorkTaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wt => wt.Description)
            .HasMaxLength(1000);

        builder.Property(wt => wt.OrderId)
            .IsRequired();

        builder.Property(wt => wt.ProductionStageId)
            .IsRequired();

        builder.Property(wt => wt.TeamId)
            .IsRequired();

        builder.Property(wt => wt.SequenceOrder)
            .IsRequired();

        builder.Property(wt => wt.Status)
            .IsRequired();

        builder.Property(wt => wt.EstimatedHours)
            .HasPrecision(10, 2);

        builder.Property(wt => wt.ActualHours)
            .HasPrecision(10, 2);

        builder.Property(wt => wt.Notes)
            .HasMaxLength(1000);

        builder.Property(wt => wt.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(wt => wt.OrderId);

        builder.HasIndex(wt => wt.FurnitureTypeId);

        builder.HasIndex(wt => wt.ProductionStageId);

        builder.HasIndex(wt => wt.TeamId);

        builder.HasIndex(wt => wt.AssignedWorkerId);

        builder.HasIndex(wt => wt.Status);

        builder.HasIndex(wt => new { wt.OrderId, wt.SequenceOrder });

        // Soft delete query filter
        builder.HasQueryFilter(wt => !wt.IsDeleted);

        // Relationships
        builder.HasOne(wt => wt.Order)
            .WithMany(o => o.WorkTasks)
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.FurnitureType)
            .WithMany(ft => ft.WorkTasks)
            .HasForeignKey(wt => wt.FurnitureTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(wt => wt.ProductionStage)
            .WithMany(ps => ps.WorkTasks)
            .HasForeignKey(wt => wt.ProductionStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.Team)
            .WithMany(t => t.WorkTasks)
            .HasForeignKey(wt => wt.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.AssignedWorker)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(wt => wt.AssignedWorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
