using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(al => al.Id);

        builder.Property(al => al.UserId)
            .IsRequired();

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityId)
            .IsRequired();

        builder.Property(al => al.OldValues)
            .HasMaxLength(4000);

        builder.Property(al => al.NewValues)
            .HasMaxLength(4000);

        builder.Property(al => al.Timestamp)
            .IsRequired();

        builder.Property(al => al.IpAddress)
            .HasMaxLength(50);

        builder.Property(al => al.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(al => al.UserId);

        builder.HasIndex(al => al.EntityName);

        builder.HasIndex(al => al.EntityId);

        builder.HasIndex(al => al.Action);

        builder.HasIndex(al => al.Timestamp);

        builder.HasIndex(al => new { al.EntityName, al.EntityId });

        // Note: AuditLog does not inherit from BaseAuditableEntity, so no soft delete filter

        // Relationships
        builder.HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
