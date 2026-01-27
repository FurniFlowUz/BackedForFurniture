using FurniFlowUz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniFlowUz.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.PositionId)
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.ActiveTasks)
            .HasDefaultValue(0);

        builder.Property(e => e.CompletedTasks)
            .HasDefaultValue(0);

        builder.Property(e => e.OnTimePercent)
            .HasColumnType("decimal(5,2)");

        // Indexes
        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.HasIndex(e => e.PositionId);

        builder.HasIndex(e => e.DepartmentId);

        builder.HasIndex(e => e.IsActive);

        // Relationships
        builder.HasOne(e => e.User)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
