using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v163 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvestorTypeId",
                table: "Investor",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor",
                column: "InvestorTypeId",
                principalTable: "InvestorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvestorTypeId",
                table: "Investor",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor",
                column: "InvestorTypeId",
                principalTable: "InvestorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
