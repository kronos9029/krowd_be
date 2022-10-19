using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v152 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOverDue",
                table: "Stage",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "PeriodRevenueHistory",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StageTotalAmount",
                table: "PeriodRevenueHistory",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SharedAmount",
                table: "PeriodRevenue",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverDue",
                table: "Stage");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PeriodRevenueHistory");

            migrationBuilder.DropColumn(
                name: "StageTotalAmount",
                table: "PeriodRevenueHistory");

            migrationBuilder.DropColumn(
                name: "SharedAmount",
                table: "PeriodRevenue");
        }
    }
}
