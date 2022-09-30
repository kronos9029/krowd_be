using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProjectWallet");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InvestorWallet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WalletTransaction",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "WalletTransaction",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProjectWallet",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Payment",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InvestorWallet",
                type: "bit",
                nullable: true);
        }
    }
}
