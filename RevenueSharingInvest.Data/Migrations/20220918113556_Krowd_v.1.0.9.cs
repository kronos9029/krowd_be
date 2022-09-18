using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "LastPayment",
                table: "Investment");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Investment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Investment");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Investment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPayment",
                table: "Investment",
                type: "datetime",
                nullable: true);
        }
    }
}
