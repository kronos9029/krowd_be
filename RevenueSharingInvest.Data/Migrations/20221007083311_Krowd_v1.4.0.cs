using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v140 : Migration
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

            migrationBuilder.AlterColumn<long>(
                name: "TransId",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ResponseTime",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "AccountTransaction",
                type: "datetime",
                nullable: true,
                defaultValueSql: "('0001-01-01T00:00:00.000')",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "varchar(12)",
                unicode: false,
                maxLength: 12,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Change_Package_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldUnicode: false,
                oldMaxLength: 12,
                oldComputedColumnSql: "(case when [dbo].[Change_Package_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)");

            migrationBuilder.AlterColumn<long>(
                name: "TransId",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

            migrationBuilder.AlterColumn<long>(
                name: "ResponseTime",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "AccountTransaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "('0001-01-01T00:00:00.000')");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

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
