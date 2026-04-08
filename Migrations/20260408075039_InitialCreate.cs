using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_FoodItems_FoodItemFoodId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_FoodItemFoodId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "FoodItemFoodId",
                table: "MealRecords");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_FoodItems_FoodId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_FoodId",
                table: "MealRecords");

            migrationBuilder.AddColumn<int>(
                name: "FoodItemFoodId",
                table: "MealRecords",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_FoodItemFoodId",
                table: "MealRecords",
                column: "FoodItemFoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealRecords_FoodItems_FoodItemFoodId",
                table: "MealRecords",
                column: "FoodItemFoodId",
                principalTable: "FoodItems",
                principalColumn: "FoodId");
        }
    }
}
