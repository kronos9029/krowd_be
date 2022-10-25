using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v170 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "StageTotalAmount",
            //    table: "PeriodRevenueHistory");

            //migrationBuilder.DropColumn(
            //    name: "Status",
            //    table: "PeriodRevenueHistory");

            //migrationBuilder.DropColumn(
            //    name: "UpdateBy",
            //    table: "PeriodRevenueHistory");

            //migrationBuilder.DropColumn(
            //    name: "UpdateDate",
            //    table: "PeriodRevenueHistory");

            //migrationBuilder.AddColumn<double>(
            //    name: "PaidAmount",
            //    table: "PeriodRevenue",
            //    type: "float",
            //    nullable: true,
            //    defaultValueSql: "((0))");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "ReportDate",
            //    table: "DailyReport",
            //    type: "datetime",
            //    nullable: false,
            //    defaultValueSql: "('0001-01-01T00:00:00.000')",
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "Stage",
            //    type: "varchar(8)",
            //    unicode: false,
            //    maxLength: 8,
            //    nullable: false,
            //    computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
            //    stored: false,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(8)",
            //    oldUnicode: false,
            //    oldMaxLength: 8,
            //    oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[StartDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[StartDate] AND dateadd(hour,(7),getdate())<=[EndDate]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id])<>NULL then 'PAID' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id]) IS NULL then 'OVERDUE' else 'INACTIVE' end)",
            //    oldStored: false);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "PeriodRevenue",
            //    type: "varchar(15)",
            //    unicode: false,
            //    maxLength: 15,
            //    nullable: true,
            //    computedColumnSql: "(case when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]>=[SharedAmount] then 'PAID_ENOUGH' when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]<[SharedAmount] then 'NOT_PAID_ENOUGH'  end)",
            //    stored: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(20)",
            //    oldMaxLength: 20,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "DailyReport",
            //    type: "varchar(12)",
            //    unicode: false,
            //    maxLength: 12,
            //    nullable: true,
            //    computedColumnSql: "(case when dateadd(hour,(7),getdate())<[ReportDate] then 'UNDUE' when dateadd(hour,(7),getdate())>=[ReportDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'DUE' when [UpdateDate] IS NOT NULL AND [UpdateBy] IS NOT NULL then 'REPORTED' when dateadd(hour,(7),getdate())>dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'NOT_REPORTED'  end)",
            //    stored: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(20)",
            //    oldMaxLength: 20,
            //    oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "PaidAmount",
            //    table: "PeriodRevenue");

            //migrationBuilder.AddColumn<double>(
            //    name: "StageTotalAmount",
            //    table: "PeriodRevenueHistory",
            //    type: "float",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Status",
            //    table: "PeriodRevenueHistory",
            //    type: "nvarchar(20)",
            //    maxLength: 20,
            //    nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "UpdateBy",
            //    table: "PeriodRevenueHistory",
            //    type: "uniqueidentifier",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "UpdateDate",
            //    table: "PeriodRevenueHistory",
            //    type: "datetime",
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "PeriodRevenue",
            //    type: "nvarchar(20)",
            //    maxLength: 20,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(15)",
            //    oldUnicode: false,
            //    oldMaxLength: 15,
            //    oldNullable: true,
            //    oldComputedColumnSql: "(case when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]>=[SharedAmount] then 'PAID_ENOUGH' when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]<[SharedAmount] then 'NOT_PAID_ENOUGH'  end)");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "DailyReport",
            //    type: "nvarchar(20)",
            //    maxLength: 20,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(12)",
            //    oldUnicode: false,
            //    oldMaxLength: 12,
            //    oldNullable: true,
            //    oldComputedColumnSql: "(case when dateadd(hour,(7),getdate())<[ReportDate] then 'UNDUE' when dateadd(hour,(7),getdate())>=[ReportDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'DUE' when [UpdateDate] IS NOT NULL AND [UpdateBy] IS NOT NULL then 'REPORTED' when dateadd(hour,(7),getdate())>dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'NOT_REPORTED'  end)");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "ReportDate",
            //    table: "DailyReport",
            //    type: "datetime",
            //    nullable: false,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime",
            //    oldDefaultValueSql: "('0001-01-01T00:00:00.000')");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "Stage",
            //    type: "varchar(8)",
            //    unicode: false,
            //    maxLength: 8,
            //    nullable: false,
            //    computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[StartDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[StartDate] AND dateadd(hour,(7),getdate())<=[EndDate]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id])<>NULL then 'PAID' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id]) IS NULL then 'OVERDUE' else 'INACTIVE' end)",
            //    stored: false,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(8)",
            //    oldUnicode: false,
            //    oldMaxLength: 8,
            //    oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
            //    oldStored: false);
        }
    }
}
