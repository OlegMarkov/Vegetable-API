using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vegetable.Core.Migrations
{
    public partial class SubscriptionImplementation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityTo",
                table: "Discounts",
                newName: "TrialQuantity");

            migrationBuilder.RenameColumn(
                name: "QuantityFrom",
                table: "Discounts",
                newName: "Quantity");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "SubscriptionTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Percentage", "Quantity", "TrialQuantity" },
                values: new object[] { 0, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Percentage", "Quantity", "TrialQuantity" },
                values: new object[] { 10, 3, 1 });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "IsEnabled", "Percentage", "Quantity", "TrialQuantity" },
                values: new object[,]
                {
                    { 3, true, 15, 6, 2 },
                    { 4, true, 20, 12, 3 }
                });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "IsDefault", "Name", "Price" },
                values: new object[] { "subscription.description-free", true, "subscription.title-free", 0 });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "IsEnabled", "Name", "Price" },
                values: new object[] { "subscription.description-premium", true, "subscription.title-premium", 20000 });

            migrationBuilder.InsertData(
                table: "SubscriptionTypes",
                columns: new[] { "Id", "Description", "IsDefault", "IsEnabled", "Name", "Price" },
                values: new object[] { 3, "subscription.description-ultra", false, false, "subscription.title-ultra", 50000 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubscriptionTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "SubscriptionTypes");

            migrationBuilder.RenameColumn(
                name: "TrialQuantity",
                table: "Discounts",
                newName: "QuantityTo");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Discounts",
                newName: "QuantityFrom");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionTypes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Percentage", "QuantityFrom", "QuantityTo" },
                values: new object[] { 10, 6, 11 });

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Percentage", "QuantityFrom", "QuantityTo" },
                values: new object[] { 20, 12, 9999 });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Premium subscription", "Premium", 20000 });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "IsEnabled", "Name", "Price" },
                values: new object[] { "Ultra subscription", false, "Ultra", 50000 });
        }
    }
}
