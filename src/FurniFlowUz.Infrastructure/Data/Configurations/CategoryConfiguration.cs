using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.Name);

        builder.HasIndex(c => c.IsActive);

        // Soft delete query filter
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Category)
            .HasForeignKey(o => o.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
