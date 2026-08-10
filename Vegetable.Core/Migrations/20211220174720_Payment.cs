using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Vegetable.Core.Migrations
{
    public partial class Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "User",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "SocialNetworks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Services",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "SchedulesOnDays",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Schedules",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Reservations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Owners",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndDate",
                table: "Owners",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionStartDate",
                table: "Owners",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionTypeId",
                table: "Owners",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Notifications",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Images",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Employees",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Addresses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "(now() at time zone 'utc')");

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuantityFrom = table.Column<int>(type: "integer", nullable: false),
                    QuantityTo = table.Column<int>(type: "integer", nullable: false),
                    Percentage = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ErrorCode = table.Column<string>(type: "text", nullable: true),
                    PaymentURL = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionTypeId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(now() at time zone 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_SubscriptionTypes_SubscriptionTypeId",
                        column: x => x.SubscriptionTypeId,
                        principalTable: "SubscriptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalKey = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ErrorCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    RebillId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CardId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Pan = table.Column<string>(type: "text", nullable: true),
                    ExpDate = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(now() at time zone 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentNotifications_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentNotifications_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "IsEnabled", "Percentage", "QuantityFrom", "QuantityTo" },
                values: new object[,]
                {
                    { 1, true, 10, 6, 11 },
                    { 2, true, 20, 12, 9999 }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionTypes",
                columns: new[] { "Id", "Description", "IsEnabled", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Premium subscription", true, "Premium", 20000 },
                    { 2, "Ultra subscription", false, "Ultra", 50000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Owners_SubscriptionTypeId",
                table: "Owners",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OwnerId",
                table: "Orders",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SubscriptionTypeId",
                table: "Orders",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentNotifications_OrderId",
                table: "PaymentNotifications",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentNotifications_OwnerId",
                table: "PaymentNotifications",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_SubscriptionTypes_SubscriptionTypeId",
                table: "Owners",
                column: "SubscriptionTypeId",
                principalTable: "SubscriptionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_SubscriptionTypes_SubscriptionTypeId",
                table: "Owners");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "PaymentNotifications");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "SubscriptionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Owners_SubscriptionTypeId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "SocialNetworks");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "SchedulesOnDays");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndDate",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "SubscriptionStartDate",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "SubscriptionTypeId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Addresses");
        }
    }
}
