using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class TechnicalSpecificationConfiguration : IEntityTypeConfiguration<TechnicalSpecification>
{
    public void Configure(EntityTypeBuilder<TechnicalSpecification> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Notes)
            .HasMaxLength(2000);

        builder.Property(ts => ts.IsLocked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ts => ts.FurnitureTypeId)
            .IsRequired();

        builder.Property(ts => ts.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ts => ts.FurnitureTypeId)
            .IsUnique();

        builder.HasIndex(ts => ts.IsLocked);

        // Soft delete query filter
        builder.HasQueryFilter(ts => !ts.IsDeleted);

        // Relationships
        builder.HasOne(ts => ts.FurnitureType)
            .WithOne(ft => ft.TechnicalSpecification)
            .HasForeignKey<TechnicalSpecification>(ts => ts.FurnitureTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
