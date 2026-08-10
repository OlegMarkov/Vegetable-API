using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vegetable.Core.Migrations
{
    public partial class CascadeDeleteOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Owners_OwnerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentNotifications_Owners_OwnerId",
                table: "PaymentNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_Owners_OwnerId",
                table: "PhoneNumbers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_User_UserId",
                table: "UserData");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "PaymentNotifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "Orders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Owners_OwnerId",
                table: "Orders",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentNotifications_Owners_OwnerId",
                table: "PaymentNotifications",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_Owners_OwnerId",
                table: "PhoneNumbers",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_User_UserId",
                table: "UserData",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Owners_OwnerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentNotifications_Owners_OwnerId",
                table: "PaymentNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_Owners_OwnerId",
                table: "PhoneNumbers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_User_UserId",
                table: "UserData");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "PaymentNotifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Owners_OwnerId",
                table: "Orders",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentNotifications_Owners_OwnerId",
                table: "PaymentNotifications",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_Owners_OwnerId",
                table: "PhoneNumbers",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_User_UserId",
                table: "UserData",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
