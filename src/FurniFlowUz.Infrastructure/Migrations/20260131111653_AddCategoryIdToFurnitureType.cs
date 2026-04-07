using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToFurnitureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "FurnitureTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureTypes_CategoryId",
                table: "FurnitureTypes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FurnitureTypes_Categories_CategoryId",
                table: "FurnitureTypes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FurnitureTypes_Categories_CategoryId",
                table: "FurnitureTypes");

            migrationBuilder.DropIndex(
                name: "IX_FurnitureTypes_CategoryId",
                table: "FurnitureTypes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "FurnitureTypes");
        }
    }
}
