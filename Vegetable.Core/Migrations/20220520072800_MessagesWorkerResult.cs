using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class MessagesWorkerResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "NotificationMessages",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "NotificationMessages");
        }
    }
}
