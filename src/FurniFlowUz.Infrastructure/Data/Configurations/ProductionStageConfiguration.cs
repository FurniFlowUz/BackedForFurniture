using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class ProductionStageConfiguration : IEntityTypeConfiguration<ProductionStage>
{
    public void Configure(EntityTypeBuilder<ProductionStage> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ps => ps.StageType)
            .IsRequired();

        builder.Property(ps => ps.SequenceOrder)
            .IsRequired();

        builder.Property(ps => ps.EstimatedDurationHours)
            .HasPrecision(10, 2);

        builder.Property(ps => ps.Description)
            .HasMaxLength(500);

        builder.Property(ps => ps.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ps => ps.StageType);

        builder.HasIndex(ps => ps.SequenceOrder);

        builder.HasIndex(ps => ps.IsActive);

        // Soft delete query filter
        builder.HasQueryFilter(ps => !ps.IsDeleted);

        // Relationships
        builder.HasMany(ps => ps.WorkTasks)
            .WithOne(wt => wt.ProductionStage)
            .HasForeignKey(wt => wt.ProductionStageId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Data - Production stages for furniture manufacturing
        builder.HasData(
            new ProductionStage
            {
                Id = 1,
                Name = "Razmer",
                StageType = ProductionStageType.Sawing,
                SequenceOrder = 1,
                EstimatedDurationHours = 2,
                Description = "O'lchov va kesish ishlari",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 2,
                Name = "Arra",
                StageType = ProductionStageType.Sawing,
                SequenceOrder = 2,
                EstimatedDurationHours = 3,
                Description = "Arra bilan kesish",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 3,
                Name = "Shipon",
                StageType = ProductionStageType.Routing,
                SequenceOrder = 3,
                EstimatedDurationHours = 2,
                Description = "Shipon bilan ishlash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 4,
                Name = "Pres",
                StageType = ProductionStageType.Routing,
                SequenceOrder = 4,
                EstimatedDurationHours = 2,
                Description = "Pres ishlari",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 5,
                Name = "Rover",
                StageType = ProductionStageType.Routing,
                SequenceOrder = 5,
                EstimatedDurationHours = 3,
                Description = "Rover stanogida ishlash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 6,
                Name = "Kromka",
                StageType = ProductionStageType.EdgeBanding,
                SequenceOrder = 6,
                EstimatedDurationHours = 2,
                Description = "Qirralarni yopish",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 7,
                Name = "Shkurka",
                StageType = ProductionStageType.Sanding,
                SequenceOrder = 7,
                EstimatedDurationHours = 3,
                Description = "Silliqlash ishlari",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 8,
                Name = "Pardozchi",
                StageType = ProductionStageType.Finishing,
                SequenceOrder = 8,
                EstimatedDurationHours = 2,
                Description = "Pardozlash tayyorlash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 9,
                Name = "Grunt",
                StageType = ProductionStageType.Finishing,
                SequenceOrder = 9,
                EstimatedDurationHours = 2,
                Description = "Grunt qoplash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 10,
                Name = "Grunt shkurka",
                StageType = ProductionStageType.Finishing,
                SequenceOrder = 10,
                EstimatedDurationHours = 2,
                Description = "Grunt silliqlash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 11,
                Name = "Kraska",
                StageType = ProductionStageType.Painting,
                SequenceOrder = 11,
                EstimatedDurationHours = 3,
                Description = "Bo'yash",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 12,
                Name = "Qurutish",
                StageType = ProductionStageType.Painting,
                SequenceOrder = 12,
                EstimatedDurationHours = 4,
                Description = "Quritish",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 13,
                Name = "OTK",
                StageType = ProductionStageType.QualityControl,
                SequenceOrder = 13,
                EstimatedDurationHours = 1,
                Description = "Sifat nazorati",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductionStage
            {
                Id = 14,
                Name = "Qadoqlash",
                StageType = ProductionStageType.Assembly,
                SequenceOrder = 14,
                EstimatedDurationHours = 2,
                Description = "Qadoqlash ishlari",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
