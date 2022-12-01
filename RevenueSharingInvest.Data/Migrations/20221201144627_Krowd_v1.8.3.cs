using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v183 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsOverDue",
                table: "Stage",
                type: "varchar(5)",
                unicode: false,
                maxLength: 5,
                nullable: true,
                computedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])>(0) AND [dbo].[Is_Paid_On_Stage]([Id])>(0) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldUnicode: false,
                oldMaxLength: 5,
                oldNullable: true,
                oldComputedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)",
                oldStored: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsOverDue",
                table: "Stage",
                type: "varchar(5)",
                unicode: false,
                maxLength: 5,
                nullable: true,
                computedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldUnicode: false,
                oldMaxLength: 5,
                oldNullable: true,
                oldComputedColumnSql: "(case when [dbo].[Get_Period_Revenue_History]([Id])>(0) AND [dbo].[Is_Paid_On_Stage]([Id])>(0) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)",
                oldStored: false);
        }
    }
}
