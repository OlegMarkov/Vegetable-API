using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class NotificationsSentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SendAttempts",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDateUTC",
                table: "Notifications",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendAttempts",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SentDateUTC",
                table: "Notifications");
        }
    }
}
