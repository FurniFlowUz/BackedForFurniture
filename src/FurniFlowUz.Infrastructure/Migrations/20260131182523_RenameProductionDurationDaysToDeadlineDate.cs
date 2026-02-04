using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductionDurationDaysToDeadlineDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new DeadlineDate column
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            // Step 2: Convert ProductionDurationDays to DeadlineDate
            // Calculate deadline as SignedDate + ProductionDurationDays, or CreatedAt + ProductionDurationDays if SignedDate is null
            migrationBuilder.Sql(@"
                UPDATE Contracts
                SET DeadlineDate = CASE
                    WHEN SignedDate IS NOT NULL THEN DATEADD(day, ProductionDurationDays, SignedDate)
                    ELSE DATEADD(day, ProductionDurationDays, CreatedAt)
                END
            ");

            // Step 3: Make DeadlineDate required
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeadlineDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            // Step 4: Drop old ProductionDurationDays column
            migrationBuilder.DropColumn(
                name: "ProductionDurationDays",
                table: "Contracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add back ProductionDurationDays column
            migrationBuilder.AddColumn<int>(
                name: "ProductionDurationDays",
                table: "Contracts",
                type: "int",
                nullable: true);

            // Step 2: Convert DeadlineDate back to ProductionDurationDays
            migrationBuilder.Sql(@"
                UPDATE Contracts
                SET ProductionDurationDays = DATEDIFF(day, COALESCE(SignedDate, CreatedAt), DeadlineDate)
            ");

            // Step 3: Make ProductionDurationDays required
            migrationBuilder.AlterColumn<int>(
                name: "ProductionDurationDays",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 30);

            // Step 4: Drop DeadlineDate column
            migrationBuilder.DropColumn(
                name: "DeadlineDate",
                table: "Contracts");
        }
    }
}
