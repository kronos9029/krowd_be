using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class v122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreateDate",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_User",
                table: "WalletTransaction",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_User_UserId",
                table: "WalletTransaction",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_User_UserId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_User",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AccountTransaction");
        }
    }
}
