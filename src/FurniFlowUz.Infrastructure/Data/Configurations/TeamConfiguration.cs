using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TeamLeaderId)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.Name);

        builder.HasIndex(t => t.TeamLeaderId);

        builder.HasIndex(t => t.IsActive);

        // Soft delete query filter
        builder.HasQueryFilter(t => !t.IsDeleted);

        // Relationships
        builder.HasOne(t => t.TeamLeader)
            .WithMany(u => u.LeadingTeams)
            .HasForeignKey(t => t.TeamLeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Members)
            .WithMany(u => u.TeamMemberships)
            .UsingEntity<Dictionary<string, object>>(
                "TeamMember",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Team>().WithMany().HasForeignKey("TeamId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("TeamId", "UserId"));  // Composite primary key - allows multiple members per team

        builder.HasMany(t => t.WorkTasks)
            .WithOne(wt => wt.Team)
            .HasForeignKey(wt => wt.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.WarehouseTransactions)
            .WithOne(wt => wt.Team)
            .HasForeignKey(wt => wt.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.MaterialRequests)
            .WithOne(mr => mr.Team)
            .HasForeignKey(mr => mr.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
