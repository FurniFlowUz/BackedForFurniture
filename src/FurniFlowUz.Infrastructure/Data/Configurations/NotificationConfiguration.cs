using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type)
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(n => n.UserId);

        builder.HasIndex(n => n.Role);

        builder.HasIndex(n => n.Type);

        builder.HasIndex(n => n.IsRead);

        builder.HasIndex(n => n.CreatedAt);

        builder.HasIndex(n => new { n.UserId, n.IsRead });

        // Soft delete query filter
        builder.HasQueryFilter(n => !n.IsDeleted);

        // Relationships
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
