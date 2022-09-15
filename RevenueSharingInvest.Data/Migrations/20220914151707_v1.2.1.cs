using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class v121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "AccountTransaction");

            migrationBuilder.RenameColumn(
                name: "UpdateBy",
                table: "AccountTransaction",
                newName: "PartnerClientId");

            migrationBuilder.RenameColumn(
                name: "QrCodeUrl",
                table: "AccountTransaction",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "PayUrl",
                table: "AccountTransaction",
                newName: "PayType");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AccountTransaction",
                newName: "PartnerUserId");

            migrationBuilder.RenameColumn(
                name: "Deeplink",
                table: "AccountTransaction",
                newName: "OrderType");

            migrationBuilder.AddColumn<string>(
                name: "CallbackToken",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraData",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderInfo",
                table: "AccountTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TransId",
                table: "AccountTransaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallbackToken",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "ExtraData",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "OrderInfo",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "TransId",
                table: "AccountTransaction");

            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "AccountTransaction",
                newName: "QrCodeUrl");

            migrationBuilder.RenameColumn(
                name: "PayType",
                table: "AccountTransaction",
                newName: "PayUrl");

            migrationBuilder.RenameColumn(
                name: "PartnerUserId",
                table: "AccountTransaction",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "PartnerClientId",
                table: "AccountTransaction",
                newName: "UpdateBy");

            migrationBuilder.RenameColumn(
                name: "OrderType",
                table: "AccountTransaction",
                newName: "Deeplink");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateBy",
                table: "AccountTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AccountTransaction",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AccountTransaction",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AccountTransaction",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "AccountTransaction",
                type: "datetime",
                nullable: true);
        }
    }
}
