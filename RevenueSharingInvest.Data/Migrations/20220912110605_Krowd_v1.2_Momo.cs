using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v12_Momo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Amount",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Deeplink",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerCode",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayUrl",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeUrl",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResponseTime",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ResultCode",
                table: "AccountTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "Deeplink",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "PartnerCode",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "PayUrl",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "QrCodeUrl",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "ResultCode",
                table: "AccountTransaction");
        }
    }
}
