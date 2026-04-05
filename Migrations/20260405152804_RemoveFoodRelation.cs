using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFoodRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_FoodItems_FoodId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_FoodId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "MealRecords");

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "MealRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId_MealDate",
                table: "MealRecords",
                columns: new[] { "EmployeeId", "MealTypeId", "MealDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId_MealDate",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "MealRecords");

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "MealRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId",
                table: "MealRecords",
                columns: new[] { "EmployeeId", "MealTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_FoodId",
                table: "MealRecords",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealRecords_FoodItems_FoodId",
                table: "MealRecords",
                column: "FoodId",
                principalTable: "FoodItems",
                principalColumn: "FoodId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
