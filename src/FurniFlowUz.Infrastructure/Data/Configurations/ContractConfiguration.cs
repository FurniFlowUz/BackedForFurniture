using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContractNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.CustomerId)
            .IsRequired();

        builder.Property(c => c.CategoryIds)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.AdvancePaymentAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.RemainingAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.PaymentStatus)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.DeadlineDate)
            .IsRequired();

        builder.Property(c => c.DeliveryTerms)
            .HasMaxLength(2000);

        builder.Property(c => c.PenaltyTerms)
            .HasMaxLength(2000);

        builder.Property(c => c.AdditionalNotes)
            .HasMaxLength(2000);

        builder.Property(c => c.RequiresApproval)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.ContractNumber)
            .IsUnique();

        builder.HasIndex(c => c.CustomerId);

        builder.HasIndex(c => c.Status);

        builder.HasIndex(c => c.PaymentStatus);

        // Soft delete query filter
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasOne(c => c.Customer)
            .WithMany(cu => cu.Contracts)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Contract)
            .HasForeignKey(o => o.ContractId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
