using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class UserMetaData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CID",
                table: "UserMetadata",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserMetadata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CID",
                table: "UserMetadata");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserMetadata");
        }
    }
}
