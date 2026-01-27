using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniFlowUz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskManagementAndKPIEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    FurnitureTypeId = table.Column<int>(type: "int", nullable: false),
                    TeamLeaderId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAssignments_FurnitureTypes_FurnitureTypeId",
                        column: x => x.FurnitureTypeId,
                        principalTable: "FurnitureTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAssignments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAssignments_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAssignments_Users_TeamLeaderId",
                        column: x => x.TeamLeaderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialRequestId = table.Column<int>(type: "int", nullable: false),
                    WarehouseItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedToTeamId = table.Column<int>(type: "int", nullable: true),
                    AssignedToEmployeeId = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedByUserId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialAssignments_MaterialRequests_MaterialRequestId",
                        column: x => x.MaterialRequestId,
                        principalTable: "MaterialRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialAssignments_Teams_AssignedToTeamId",
                        column: x => x.AssignedToTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialAssignments_Users_AssignedToEmployeeId",
                        column: x => x.AssignedToEmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialAssignments_Users_ReceivedByUserId",
                        column: x => x.ReceivedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialAssignments_WarehouseItems_WarehouseItemId",
                        column: x => x.WarehouseItemId,
                        principalTable: "WarehouseItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetailTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryAssignmentId = table.Column<int>(type: "int", nullable: false),
                    DetailId = table.Column<int>(type: "int", nullable: false),
                    AssignedEmployeeId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    DependsOnTaskId = table.Column<int>(type: "int", nullable: true),
                    TaskDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailTasks_CategoryAssignments_CategoryAssignmentId",
                        column: x => x.CategoryAssignmentId,
                        principalTable: "CategoryAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailTasks_DetailTasks_DependsOnTaskId",
                        column: x => x.DependsOnTaskId,
                        principalTable: "DetailTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DetailTasks_Details_DetailId",
                        column: x => x.DetailId,
                        principalTable: "Details",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetailTasks_Users_AssignedEmployeeId",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskPerformances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailTaskId = table.Column<int>(type: "int", nullable: false),
                    ActualDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    EfficiencyPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    QualityScore = table.Column<int>(type: "int", nullable: false),
                    RequiredRework = table.Column<bool>(type: "bit", nullable: false),
                    ReworkReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPerformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskPerformances_DetailTasks_DetailTaskId",
                        column: x => x.DetailTaskId,
                        principalTable: "DetailTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_FurnitureTypeId",
                table: "CategoryAssignments",
                column: "FurnitureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_OrderId",
                table: "CategoryAssignments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_Status",
                table: "CategoryAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_TeamId",
                table: "CategoryAssignments",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_TeamLeaderId",
                table: "CategoryAssignments",
                column: "TeamLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_AssignedEmployeeId",
                table: "DetailTasks",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_CategoryAssignmentId",
                table: "DetailTasks",
                column: "CategoryAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_CategoryAssignmentId_Sequence",
                table: "DetailTasks",
                columns: new[] { "CategoryAssignmentId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_DependsOnTaskId",
                table: "DetailTasks",
                column: "DependsOnTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_DetailId",
                table: "DetailTasks",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTasks_Status",
                table: "DetailTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAssignments_AssignedToEmployeeId",
                table: "MaterialAssignments",
                column: "AssignedToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAssignments_AssignedToTeamId",
                table: "MaterialAssignments",
                column: "AssignedToTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAssignments_MaterialRequestId",
                table: "MaterialAssignments",
                column: "MaterialRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAssignments_ReceivedByUserId",
                table: "MaterialAssignments",
                column: "ReceivedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAssignments_WarehouseItemId",
                table: "MaterialAssignments",
                column: "WarehouseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPerformances_DetailTaskId",
                table: "TaskPerformances",
                column: "DetailTaskId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialAssignments");

            migrationBuilder.DropTable(
                name: "TaskPerformances");

            migrationBuilder.DropTable(
                name: "DetailTasks");

            migrationBuilder.DropTable(
                name: "CategoryAssignments");
        }
    }
}
