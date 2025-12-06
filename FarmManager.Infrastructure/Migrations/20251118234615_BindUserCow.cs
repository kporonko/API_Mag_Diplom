using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BindUserCow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlaggedForSlaughter",
                table: "Cows");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Cows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId1",
                table: "Cows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cows_ApplicationUserId",
                table: "Cows",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cows_ApplicationUserId1",
                table: "Cows",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cows_Users_ApplicationUserId",
                table: "Cows",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cows_Users_ApplicationUserId1",
                table: "Cows",
                column: "ApplicationUserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cows_Users_ApplicationUserId",
                table: "Cows");

            migrationBuilder.DropForeignKey(
                name: "FK_Cows_Users_ApplicationUserId1",
                table: "Cows");

            migrationBuilder.DropIndex(
                name: "IX_Cows_ApplicationUserId",
                table: "Cows");

            migrationBuilder.DropIndex(
                name: "IX_Cows_ApplicationUserId1",
                table: "Cows");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Cows");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Cows");

            migrationBuilder.AddColumn<bool>(
                name: "IsFlaggedForSlaughter",
                table: "Cows",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
