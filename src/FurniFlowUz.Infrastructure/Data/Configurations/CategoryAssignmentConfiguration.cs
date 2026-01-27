using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class CategoryAssignmentConfiguration : IEntityTypeConfiguration<CategoryAssignment>
{
    public void Configure(EntityTypeBuilder<CategoryAssignment> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.Status)
            .IsRequired();

        builder.Property(ca => ca.AssignedAt)
            .IsRequired();

        builder.Property(ca => ca.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(ca => ca.Order)
            .WithMany()
            .HasForeignKey(ca => ca.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.FurnitureType)
            .WithMany()
            .HasForeignKey(ca => ca.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.TeamLeader)
            .WithMany()
            .HasForeignKey(ca => ca.TeamLeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.Team)
            .WithMany()
            .HasForeignKey(ca => ca.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ca => ca.DetailTasks)
            .WithOne(dt => dt.CategoryAssignment)
            .HasForeignKey(dt => dt.CategoryAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ca => ca.OrderId);
        builder.HasIndex(ca => ca.TeamLeaderId);
        builder.HasIndex(ca => ca.Status);
    }
}
