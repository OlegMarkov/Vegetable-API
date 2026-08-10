using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vegetable.Core.Migrations
{
    public partial class Schedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BreakEndTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BreakStartTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RepeatPeriodType",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RepeatValue",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "WeekDays",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "WorkEndTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "WorkStartTime",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "OffDays",
                table: "Schedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnDays",
                table: "Schedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleType",
                table: "Schedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SchedulesOnDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    WorkStartTime = table.Column<TimeSpan>(nullable: false),
                    WorkEndTime = table.Column<TimeSpan>(nullable: false),
                    BreakStartTime = table.Column<TimeSpan>(nullable: false),
                    BreakEndTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulesOnDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulesOnDays_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulesOnDays_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulesOnDays_OwnerId",
                table: "SchedulesOnDays",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulesOnDays_ScheduleId",
                table: "SchedulesOnDays",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "SchedulesOnDays");

            migrationBuilder.DropColumn(
                name: "OffDays",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "OnDays",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ScheduleType",
                table: "Schedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakEndTime",
                table: "Schedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakStartTime",
                table: "Schedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "RepeatPeriodType",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RepeatValue",
                table: "Schedules",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int[]>(
                name: "WeekDays",
                table: "Schedules",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkEndTime",
                table: "Schedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkStartTime",
                table: "Schedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
