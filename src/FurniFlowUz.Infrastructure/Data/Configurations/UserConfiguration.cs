using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Role)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Role);

        // Soft delete query filter
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Relationships
        builder.HasMany(u => u.AssignedOrdersAsConstructor)
            .WithOne(o => o.AssignedConstructor)
            .HasForeignKey(o => o.AssignedConstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignedOrdersAsProductionManager)
            .WithOne(o => o.AssignedProductionManager)
            .HasForeignKey(o => o.AssignedProductionManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.LeadingTeams)
            .WithOne(t => t.TeamLeader)
            .HasForeignKey(t => t.TeamLeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TeamMemberships)
            .WithMany(t => t.Members)
            .UsingEntity<Dictionary<string, object>>(
                "TeamMember",
                j => j.HasOne<Team>().WithMany().HasForeignKey("TeamId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade));

        builder.HasMany(u => u.AssignedTasks)
            .WithOne(wt => wt.AssignedWorker)
            .HasForeignKey(wt => wt.AssignedWorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.WarehouseTransactions)
            .WithOne(wt => wt.CreatedByUser)
            .HasForeignKey(wt => wt.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.MaterialRequests)
            .WithOne(mr => mr.RequestedByUser)
            .HasForeignKey(mr => mr.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ConfirmedMaterialRequests)
            .WithOne(mr => mr.ConfirmedByUser)
            .HasForeignKey(mr => mr.ConfirmedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AuditLogs)
            .WithOne(al => al.User)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
