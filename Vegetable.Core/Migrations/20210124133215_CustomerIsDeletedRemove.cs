using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class CustomerIsDeletedRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customers",
                type: "boolean",
                nullable: true);
        }
    }
}
