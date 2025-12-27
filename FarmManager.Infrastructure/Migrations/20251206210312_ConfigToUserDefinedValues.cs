using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigToUserDefinedValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "FoodConsumptions",
                newName: "PricePerKg");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultFoodPrice",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMeatPrice",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMilkPrice",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerLiter",
                table: "MilkYields",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultFoodPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultMeatPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultMilkPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PricePerLiter",
                table: "MilkYields");

            migrationBuilder.RenameColumn(
                name: "PricePerKg",
                table: "FoodConsumptions",
                newName: "Cost");
        }
    }
}
