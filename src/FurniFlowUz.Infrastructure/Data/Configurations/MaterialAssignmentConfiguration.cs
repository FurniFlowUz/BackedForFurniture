using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class MaterialAssignmentConfiguration : IEntityTypeConfiguration<MaterialAssignment>
{
    public void Configure(EntityTypeBuilder<MaterialAssignment> builder)
    {
        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.Quantity)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ma => ma.AssignedAt)
            .IsRequired();

        builder.Property(ma => ma.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(ma => ma.MaterialRequest)
            .WithMany()
            .HasForeignKey(ma => ma.MaterialRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.WarehouseItem)
            .WithMany()
            .HasForeignKey(ma => ma.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.AssignedToTeam)
            .WithMany()
            .HasForeignKey(ma => ma.AssignedToTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.AssignedToEmployee)
            .WithMany()
            .HasForeignKey(ma => ma.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.ReceivedByUser)
            .WithMany()
            .HasForeignKey(ma => ma.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ma => ma.MaterialRequestId);
        builder.HasIndex(ma => ma.AssignedToTeamId);
        builder.HasIndex(ma => ma.AssignedToEmployeeId);
    }
}
