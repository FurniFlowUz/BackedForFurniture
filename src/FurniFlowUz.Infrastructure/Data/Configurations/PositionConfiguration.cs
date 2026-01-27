using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(p => p.Name);

        // Relationships
        builder.HasMany(p => p.Employees)
            .WithOne(e => e.Position)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
