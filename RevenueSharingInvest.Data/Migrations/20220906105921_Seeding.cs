using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Seeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "InvestorType",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("ec92ef2a-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Mua và nắm giữ dài hạn, thường là các HOLDER, họ có niềm tin vào tiền điện tử cũng như công nghệ Blockchain.", false, "LONG_INVESTOR", null, null },
                    { new Guid("07c55f72-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Những người trẻ tuổi, quan tâm đến những thứ mới lạ có thể mang lại sự thay đổi cho tương lai và đặc biệt thích về các công nghệ.", false, "HOBBYIST_INVESTOR", null, null },
                    { new Guid("ca4e68cc-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Những người đầu tư ngắn hạn, thường là Day Trader, họ tìm kiếm lợi nhuận từ những biến động của thị trường trong thời gian ngắn, thường sử dụng margin để gia tăng lợi nhuận.", false, "SHORT_INVESTOR", null, null },
                    { new Guid("175389b8-f795-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Họ thường là những người trẻ tuổi, có kiến thức tốt và thấu hiểu bản thân, nhìn thì trường bằng con mắt đa chiều và cũng rất thông minh.", false, "STRATEGIC_INVESTOR", null, null }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Business manager", false, "BUSINESS_MANAGER", null, null },
                    { new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Investor", false, "INVESTOR", null, null },
                    { new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Project owner", false, "PROJECT_OWNER", null, null },
                    { new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Krowd's admin", false, "ADMIN", null, null }
                });

            migrationBuilder.InsertData(
                table: "WalletType",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Mode", "Name", "Type", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("4e24a3d5-9aed-4db2-87f5-bd69c55899b7"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví thanh toán dự án của Investor", false, "INVESTOR", "PROJECT_PAYMENT_WALLET", "I4", null, null },
                    { new Guid("ba9baf2f-b063-41a2-808b-a452afa3e57f"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví tạm ứng của Investor", false, "INVESTOR", "ADVANCE_WALLET", "I3", null, null },
                    { new Guid("c485dc8b-b61d-4de9-8939-4e765a3f9e7d"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví thu tiền của Investor", false, "INVESTOR", "INVESTOR_COLLECTING_WALLET", "I5", null, null },
                    { new Guid("a036a7d2-980b-41b2-8ec2-06bff8782b66"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví đầu tư dự án của Business", false, "BUSINESS", "PROJECT_INVESTMENT_WALLET", "B3", null, null },
                    { new Guid("67453687-e268-4f32-8fb8-0e7c77de2c71"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví tạm thời của Investor", false, "INVESTOR", "TEMPORARY_WALLET", "I1", null, null },
                    { new Guid("0568667c-1e13-440b-8d4a-077288aa9919"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví thanh toán chung của Business", false, "BUSINESS", "UNIVERSAL_PAYMENT_WALLET", "B1", null, null },
                    { new Guid("05d47eb3-06a5-4718-a46a-d62494dee371"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví thanh toán doanh nghiệp của Business", false, "BUSINESS", "BUSINESS_PAYMENT_WALLET", "B2", null, null },
                    { new Guid("e3b41a08-135b-4fb3-bf1f-4d3675d39f96"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví đầu tư chung của Investors", false, "INVESTOR", "GENERAL_INVESTMENT_WALLET", "I2", null, null },
                    { new Guid("d7ed0979-285f-4ec0-9f6b-ae95fcfa9207"), null, new DateTime(2022, 9, 6, 17, 59, 20, 900, DateTimeKind.Local).AddTicks(1817), "Ví thu tiền của Business", false, "BUSINESS", "BUSINESS_COLLECTING_WALLET", "B4", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("07c55f72-f794-11ec-b939-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("175389b8-f795-11ec-b939-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ca4e68cc-f794-11ec-b939-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "InvestorType",
                keyColumn: "Id",
                keyValue: new Guid("ec92ef2a-f794-11ec-b939-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("0568667c-1e13-440b-8d4a-077288aa9919"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("05d47eb3-06a5-4718-a46a-d62494dee371"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("4e24a3d5-9aed-4db2-87f5-bd69c55899b7"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("67453687-e268-4f32-8fb8-0e7c77de2c71"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("a036a7d2-980b-41b2-8ec2-06bff8782b66"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("ba9baf2f-b063-41a2-808b-a452afa3e57f"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("c485dc8b-b61d-4de9-8939-4e765a3f9e7d"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("d7ed0979-285f-4ec0-9f6b-ae95fcfa9207"));

            migrationBuilder.DeleteData(
                table: "WalletType",
                keyColumn: "Id",
                keyValue: new Guid("e3b41a08-135b-4fb3-bf1f-4d3675d39f96"));
        }
    }
}
