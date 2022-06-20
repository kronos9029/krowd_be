using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v143 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "VoucherItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItem",
                column: "VoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem");

            migrationBuilder.DropIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItem");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VoucherItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem",
                column: "VoucherId");
        }
    }
}
