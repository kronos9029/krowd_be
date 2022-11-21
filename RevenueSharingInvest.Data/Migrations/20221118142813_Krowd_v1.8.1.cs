using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v181 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WithdrawRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "PaidAmount",
                table: "PeriodRevenue",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Stage",
                type: "varchar(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])>[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])<=[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DONE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldUnicode: false,
                oldMaxLength: 8,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
                oldStored: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WithdrawRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PaidAmount",
                table: "PeriodRevenue",
                type: "float",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Stage",
                type: "varchar(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldUnicode: false,
                oldMaxLength: 8,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])>[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])<=[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DONE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)",
                oldStored: false);
        }
    }
}
