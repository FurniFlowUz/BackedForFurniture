using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class DetailConfiguration : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Width)
            .HasPrecision(10, 2);

        builder.Property(d => d.Height)
            .HasPrecision(10, 2);

        builder.Property(d => d.Thickness)
            .HasPrecision(10, 2);

        builder.Property(d => d.Quantity)
            .IsRequired();

        builder.Property(d => d.FurnitureTypeId)
            .IsRequired();

        builder.Property(d => d.Material)
            .HasMaxLength(200);

        builder.Property(d => d.Notes)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(d => d.FurnitureTypeId);

        // Soft delete query filter
        builder.HasQueryFilter(d => !d.IsDeleted);

        // Relationships
        builder.HasOne(d => d.FurnitureType)
            .WithMany(ft => ft.Details)
            .HasForeignKey(d => d.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
