using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vegetable.Core.Migrations
{
    public partial class CustomerSendConfirmationSms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendConfirmationSms",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendConfirmationSms",
                table: "Customers");
        }
    }
}
