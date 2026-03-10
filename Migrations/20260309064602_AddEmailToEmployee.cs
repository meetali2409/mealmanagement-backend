using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId_MealDate",
                table: "MealRecords");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId",
                table: "MealRecords",
                columns: new[] { "EmployeeId", "MealTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_EmployeeId_MealTypeId_MealDate",
                table: "MealRecords",
                columns: new[] { "EmployeeId", "MealTypeId", "MealDate" },
                unique: true);
        }
    }
}
