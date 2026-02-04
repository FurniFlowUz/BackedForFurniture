using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCategoryIdWithOrderCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FurnitureTypes_Categories_CategoryId",
                table: "FurnitureTypes");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "FurnitureTypes",
                newName: "OrderCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_FurnitureTypes_CategoryId",
                table: "FurnitureTypes",
                newName: "IX_FurnitureTypes_OrderCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FurnitureTypes_OrderCategories_OrderCategoryId",
                table: "FurnitureTypes",
                column: "OrderCategoryId",
                principalTable: "OrderCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FurnitureTypes_OrderCategories_OrderCategoryId",
                table: "FurnitureTypes");

            migrationBuilder.RenameColumn(
                name: "OrderCategoryId",
                table: "FurnitureTypes",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_FurnitureTypes_OrderCategoryId",
                table: "FurnitureTypes",
                newName: "IX_FurnitureTypes_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FurnitureTypes_Categories_CategoryId",
                table: "FurnitureTypes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
