using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class DrawingConfiguration : IEntityTypeConfiguration<Drawing>
{
    public void Configure(EntityTypeBuilder<Drawing> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.FurnitureTypeId)
            .IsRequired();

        builder.Property(d => d.UploadedAt)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(d => d.FurnitureTypeId);

        // Soft delete query filter
        builder.HasQueryFilter(d => !d.IsDeleted);

        // Relationships
        builder.HasOne(d => d.FurnitureType)
            .WithMany(ft => ft.Drawings)
            .HasForeignKey(d => d.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
