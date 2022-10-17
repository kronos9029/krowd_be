using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v150 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodRevenue_Project",
                table: "PeriodRevenue");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodRevenue_Stage_StageId",
                table: "PeriodRevenue");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Business",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Stage_Project",
                table: "Stage");

            migrationBuilder.AddColumn<double>(
                name: "RemainingMaximumPayableAmount",
                table: "Project",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RemainingPayableAmount",
                table: "Project",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodRevenue_Project",
                table: "PeriodRevenue",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodRevenue_Stage_StageId",
                table: "PeriodRevenue",
                column: "StageId",
                principalTable: "Stage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Business",
                table: "Project",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stage_Project",
                table: "Stage",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodRevenue_Project",
                table: "PeriodRevenue");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodRevenue_Stage_StageId",
                table: "PeriodRevenue");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Business",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Stage_Project",
                table: "Stage");

            migrationBuilder.DropColumn(
                name: "RemainingMaximumPayableAmount",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "RemainingPayableAmount",
                table: "Project");

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodRevenue_Project",
                table: "PeriodRevenue",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodRevenue_Stage_StageId",
                table: "PeriodRevenue",
                column: "StageId",
                principalTable: "Stage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Business",
                table: "Project",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stage_Project",
                table: "Stage",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
