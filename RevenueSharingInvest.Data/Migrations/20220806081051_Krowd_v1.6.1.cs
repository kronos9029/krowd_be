using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v161 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseDate",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "MaxForPurchasing",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "MinForPurchasing",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "OpenDate",
                table: "Package");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Package");

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseDate",
                table: "Package",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxForPurchasing",
                table: "Package",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinForPurchasing",
                table: "Package",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenDate",
                table: "Package",
                type: "datetime",
                nullable: true);
        }
    }
}
