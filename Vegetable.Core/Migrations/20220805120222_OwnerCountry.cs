using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vegetable.Core.Migrations
{
    public partial class OwnerCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Owners",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Owners");
        }
    }
}
