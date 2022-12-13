using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v184 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "varchar(12)",
                unicode: false,
                maxLength: 12,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<=[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])>[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) then 'BLOCKED' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldUnicode: false,
                oldMaxLength: 12,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])>[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) then 'BLOCKED' else 'INACTIVE' end)",
                oldStored: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "varchar(12)",
                unicode: false,
                maxLength: 12,
                nullable: false,
                computedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])>[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) then 'BLOCKED' else 'INACTIVE' end)",
                stored: false,
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldUnicode: false,
                oldMaxLength: 12,
                oldComputedColumnSql: "(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<=[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])>[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) then 'BLOCKED' else 'INACTIVE' end)",
                oldStored: false);
        }
    }
}
