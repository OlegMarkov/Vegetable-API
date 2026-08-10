using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class OwnerTimeZone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Owners",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_OwnerId",
                table: "User",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Owners_OwnerId",
                table: "User",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Owners_OwnerId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_OwnerId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Owners");
        }
    }
}
