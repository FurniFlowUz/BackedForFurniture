using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class DetailTaskConfiguration : IEntityTypeConfiguration<DetailTask>
{
    public void Configure(EntityTypeBuilder<DetailTask> builder)
    {
        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.TaskDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(dt => dt.Status)
            .IsRequired();

        builder.Property(dt => dt.Sequence)
            .IsRequired();

        builder.Property(dt => dt.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(dt => dt.CategoryAssignment)
            .WithMany(ca => ca.DetailTasks)
            .HasForeignKey(dt => dt.CategoryAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dt => dt.Detail)
            .WithMany()
            .HasForeignKey(dt => dt.DetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dt => dt.AssignedEmployee)
            .WithMany()
            .HasForeignKey(dt => dt.AssignedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dt => dt.DependsOnTask)
            .WithMany()
            .HasForeignKey(dt => dt.DependsOnTaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(dt => dt.Performance)
            .WithOne(tp => tp.DetailTask)
            .HasForeignKey<TaskPerformance>(tp => tp.DetailTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(dt => dt.CategoryAssignmentId);
        builder.HasIndex(dt => dt.AssignedEmployeeId);
        builder.HasIndex(dt => dt.Status);
        builder.HasIndex(dt => new { dt.CategoryAssignmentId, dt.Sequence });
    }
}
