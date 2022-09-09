using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Krowd_v101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_Project",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Package");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Package",
                type: "uniqueidentifier",
                nullable: false,
                //defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("15f8f9bc-e701-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("180c2784-e700-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("29e3709e-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("4f492fa4-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("6e39f240-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("98d579ca-0685-11ed-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("b289b3a4-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("d1a18b54-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("fc24cff0-e6fd-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("07c55f72-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("175389b8-f795-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ca4e68cc-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ec92ef2a-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("21d77b9a-f792-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("0568667c-1e13-440b-8d4a-077288aa9919"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("05d47eb3-06a5-4718-a46a-d62494dee371"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("4e24a3d5-9aed-4db2-87f5-bd69c55899b7"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("67453687-e268-4f32-8fb8-0e7c77de2c71"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("a036a7d2-980b-41b2-8ec2-06bff8782b66"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("ba9baf2f-b063-41a2-808b-a452afa3e57f"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("c485dc8b-b61d-4de9-8939-4e765a3f9e7d"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("d7ed0979-285f-4ec0-9f6b-ae95fcfa9207"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("e3b41a08-135b-4fb3-bf1f-4d3675d39f96"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 18, 41, 44, 420, DateTimeKind.Local).AddTicks(4908));

            migrationBuilder.AddForeignKey(
                name: "FK_Package_Project",
                table: "Package",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_Project",
                table: "Package");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Package",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Package",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("15f8f9bc-e701-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("180c2784-e700-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("29e3709e-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("4f492fa4-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("6e39f240-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("98d579ca-0685-11ed-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("b289b3a4-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("d1a18b54-e6ff-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("fc24cff0-e6fd-11ec-8fea-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("07c55f72-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("175389b8-f795-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ca4e68cc-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ec92ef2a-f794-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("21d77b9a-f792-11ec-b939-0242ac120002"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("0568667c-1e13-440b-8d4a-077288aa9919"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("05d47eb3-06a5-4718-a46a-d62494dee371"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("4e24a3d5-9aed-4db2-87f5-bd69c55899b7"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("67453687-e268-4f32-8fb8-0e7c77de2c71"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("a036a7d2-980b-41b2-8ec2-06bff8782b66"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("ba9baf2f-b063-41a2-808b-a452afa3e57f"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("c485dc8b-b61d-4de9-8939-4e765a3f9e7d"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("d7ed0979-285f-4ec0-9f6b-ae95fcfa9207"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("e3b41a08-135b-4fb3-bf1f-4d3675d39f96"),
                column: "CreateDate",
                value: new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.AddForeignKey(
                name: "FK_Package_Project",
                table: "Package",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
