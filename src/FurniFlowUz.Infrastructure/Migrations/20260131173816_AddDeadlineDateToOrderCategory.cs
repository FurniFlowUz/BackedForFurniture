using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeadlineDateToOrderCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineDate",
                table: "OrderCategories",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadlineDate",
                table: "OrderCategories");
        }
    }
}
