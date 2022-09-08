using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Project",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "('0001-01-01T00:00:00.000')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Project",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "('0001-01-01T00:00:00.000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Project",
                type: "datetime",
                nullable: false,
                defaultValueSql: "('0001-01-01T00:00:00.000')",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Project",
                type: "datetime",
                nullable: false,
                defaultValueSql: "('0001-01-01T00:00:00.000')",
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
