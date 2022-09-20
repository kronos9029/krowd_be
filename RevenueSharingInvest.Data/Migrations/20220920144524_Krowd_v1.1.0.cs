using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Investor",
                table: "Investment");

            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Package",
                table: "Investment");

            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Project",
                table: "Investment");

            migrationBuilder.DropIndex(
                name: "IX_Investment_InvestorId",
                table: "Investment");

            migrationBuilder.DropIndex(
                name: "IX_Investment_PackageId",
                table: "Investment");

            migrationBuilder.DropIndex(
                name: "IX_Investment_ProjectId",
                table: "Investment");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PackageId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InvestorId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "PackageId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvestorId",
                table: "Investment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_InvestorId",
                table: "Investment",
                column: "InvestorId");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_PackageId",
                table: "Investment",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_ProjectId",
                table: "Investment",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Investor",
                table: "Investment",
                column: "InvestorId",
                principalTable: "Investor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Package",
                table: "Investment",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Project",
                table: "Investment",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
