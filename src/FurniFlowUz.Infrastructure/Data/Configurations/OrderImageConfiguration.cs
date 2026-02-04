using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class OrderImageConfiguration : IEntityTypeConfiguration<OrderImage>
{
    public void Configure(EntityTypeBuilder<OrderImage> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.OrderId)
            .IsRequired();

        builder.Property(oi => oi.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(oi => oi.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(oi => oi.FileSize)
            .IsRequired();

        builder.Property(oi => oi.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(oi => oi.ImageType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(oi => oi.UploadedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ImageType);

        // Soft delete query filter
        builder.HasQueryFilter(oi => !oi.IsDeleted);

        // Relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderImages)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Uploader)
            .WithMany()
            .HasForeignKey(oi => oi.UploadedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
