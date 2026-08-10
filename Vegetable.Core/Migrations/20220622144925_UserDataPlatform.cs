using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vegetable.Core.Migrations
{
    public partial class UserDataPlatform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "UserData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "NotificationMessages",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "NotificationMessages");
        }
    }
}
