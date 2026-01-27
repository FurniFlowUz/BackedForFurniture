using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.PhoneNumber);

        builder.HasIndex(c => c.Email);

        // Soft delete query filter
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasMany(c => c.Contracts)
            .WithOne(ct => ct.Customer)
            .HasForeignKey(ct => ct.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
