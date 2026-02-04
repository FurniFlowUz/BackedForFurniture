using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // OrderCategories table already created via SQL script
            // This migration is empty because the table exists
            // Only updating the EF Core model snapshot
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCategories");
        }
    }
}
