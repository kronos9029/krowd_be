using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v164 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor");

            migrationBuilder.DropIndex(
                name: "IX_Investor_InvestorTypeId",
                table: "Investor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Investor_InvestorTypeId",
                table: "Investor",
                column: "InvestorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investor_InvestorType",
                table: "Investor",
                column: "InvestorTypeId",
                principalTable: "InvestorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
