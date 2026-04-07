using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "MealRecords");

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "MealRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_FoodItems_FoodItemFoodId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_FoodItemFoodId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "FoodItemFoodId",
                table: "MealRecords");

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "MealRecords",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
