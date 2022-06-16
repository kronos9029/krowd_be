using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v142 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherItem_Voucher",
                table: "VoucherItem");

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

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherItem_Voucher",
                table: "VoucherItem",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherItem_Voucher",
                table: "VoucherItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "VoucherItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItem",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherItem_Voucher",
                table: "VoucherItem",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
