using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v153 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Stage",
                type: "varchar(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[StartDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[StartDate] AND dateadd(hour,(7),getdate())<=[EndDate]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id])<>NULL then 'PAID' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id]) IS NULL then 'OVERDUE' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IsOverDue",
                table: "Stage",
                type: "varchar(5)",
                unicode: false,
                maxLength: 5,
                nullable: true,
                computedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)",
                stored: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "varchar(12)",
                unicode: false,
                maxLength: 12,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldUnicode: false,
                oldMaxLength: 12,
                oldComputedColumnSql: "(case when [dbo].[Change_Package_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Change_Package_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)",
                oldStored: false);

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    InvoiceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Bill",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_ProjectId",
                table: "Bill",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Stage",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldUnicode: false,
                oldMaxLength: 8,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[StartDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[StartDate] AND dateadd(hour,(7),getdate())<=[EndDate]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id])<>NULL then 'PAID' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id]) IS NULL then 'OVERDUE' else 'INACTIVE' end)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOverDue",
                table: "Stage",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldUnicode: false,
                oldMaxLength: 5,
                oldNullable: true,
                oldComputedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)");

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
                oldType: "varchar(12)",
                oldUnicode: false,
                oldMaxLength: 12,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)",
                oldStored: false);
        }
    }
}
