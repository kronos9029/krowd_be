using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Seeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Field",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("180c2784-e700-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "School, Tutor, Learning tools", false, "Education", null, null },
                    { new Guid("15f8f9bc-e701-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Spa, Cosmetic, Hair salon", false, "Beauty", null, null },
                    { new Guid("fc24cff0-e6fd-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Restaurant, Food Court, Culinary", false, "Food", null, null },
                    { new Guid("6e39f240-e6ff-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Functional foods, Clean food", false, "Health", null, null },
                    { new Guid("d1a18b54-e6ff-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Gym, Sportwear, Exercise machines", false, "Fitness", null, null },
                    { new Guid("29e3709e-e6ff-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Restaurant, Drink Court, Culinary, Cafe", false, "Drink", null, null },
                    { new Guid("4f492fa4-e6ff-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Clothes, Shoes, Bags", false, "Fashion", null, null },
                    { new Guid("b289b3a4-e6ff-11ec-8fea-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Drug, Medical devices", false, "Medical", null, null },
                    { new Guid("98d579ca-0685-11ed-b939-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Food, Drink, Personal belongings", false, "Grocery", null, null }
                });

            migrationBuilder.InsertData(
                table: "InvestorType",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("ca4e68cc-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Những người đầu tư ngắn hạn, thường là Day Trader, họ tìm kiếm lợi nhuận từ những biến động của thị trường trong thời gian ngắn, thường sử dụng margin để gia tăng lợi nhuận.", false, "SHORT_INVESTOR", null, null },
                    { new Guid("175389b8-f795-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Họ thường là những người trẻ tuổi, có kiến thức tốt và thấu hiểu bản thân, nhìn thì trường bằng con mắt đa chiều và cũng rất thông minh.", false, "STRATEGIC_INVESTOR", null, null },
                    { new Guid("ec92ef2a-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Mua và nắm giữ dài hạn, thường là các HOLDER, họ có niềm tin vào tiền điện tử cũng như công nghệ Blockchain.", false, "LONG_INVESTOR", null, null },
                    { new Guid("07c55f72-f794-11ec-b939-0242ac120002"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Những người trẻ tuổi, quan tâm đến những thứ mới lạ có thể mang lại sự thay đổi cho tương lai và đặc biệt thích về các công nghệ.", false, "HOBBYIST_INVESTOR", null, null }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Business manager", false, "BUSINESS_MANAGER", null, null },
                    { new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Investor", false, "INVESTOR", null, null },
                    { new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Project owner", false, "PROJECT_OWNER", null, null },
                    { new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Krowd's admin", false, "ADMIN", null, null }
                });

            migrationBuilder.InsertData(
                table: "WalletType",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "IsDeleted", "Mode", "Name", "Type", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("05d47eb3-06a5-4718-a46a-d62494dee371"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví thanh toán doanh nghiệp của Business", false, "BUSINESS", "BUSINESS_PAYMENT_WALLET", "B2", null, null },
                    { new Guid("a036a7d2-980b-41b2-8ec2-06bff8782b66"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví đầu tư dự án của Business", false, "BUSINESS", "PROJECT_INVESTMENT_WALLET", "B3", null, null },
                    { new Guid("0568667c-1e13-440b-8d4a-077288aa9919"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví thanh toán chung của Business", false, "BUSINESS", "UNIVERSAL_PAYMENT_WALLET", "B1", null, null },
                    { new Guid("67453687-e268-4f32-8fb8-0e7c77de2c71"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví tạm thời của Investor", false, "INVESTOR", "TEMPORARY_WALLET", "I1", null, null },
                    { new Guid("e3b41a08-135b-4fb3-bf1f-4d3675d39f96"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví đầu tư chung của Investors", false, "INVESTOR", "GENERAL_INVESTMENT_WALLET", "I2", null, null },
                    { new Guid("c485dc8b-b61d-4de9-8939-4e765a3f9e7d"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví thu tiền của Investor", false, "INVESTOR", "INVESTOR_COLLECTING_WALLET", "I5", null, null },
                    { new Guid("ba9baf2f-b063-41a2-808b-a452afa3e57f"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví tạm ứng của Investor", false, "INVESTOR", "ADVANCE_WALLET", "I3", null, null },
                    { new Guid("4e24a3d5-9aed-4db2-87f5-bd69c55899b7"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví thanh toán dự án của Investor", false, "INVESTOR", "PROJECT_PAYMENT_WALLET", "I4", null, null },
                    { new Guid("d7ed0979-285f-4ec0-9f6b-ae95fcfa9207"), null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), "Ví thu tiền của Business", false, "BUSINESS", "BUSINESS_COLLECTING_WALLET", "B4", null, null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BankAccount", "BankName", "BusinessId", "City", "CreateBy", "CreateDate", "DateOfBirth", "Description", "District", "Email", "FirstName", "Gender", "IdCard", "Image", "IsDeleted", "LastName", "PhoneNum", "RoleId", "Status", "TaxIdentificationNumber", "UpdateBy", "UpdateDate" },
                values: new object[] { new Guid("21d77b9a-f792-11ec-b939-0242ac120002"), null, null, null, null, null, null, new DateTime(2022, 9, 8, 13, 35, 0, 889, DateTimeKind.Local).AddTicks(3185), null, "Admin 1", null, "krowd.dev.2022@gmail.com", "Krowd's", "LGBT", null, "https://firebasestorage.googleapis.com/v0/b/revenuesharinginvest-44354.appspot.com/o/User%2Favt%20Kh%C3%A1nh.jpg?alt=media&token=0940aab1-edaf-443b-ad83-1d14cb8dff1f", false, "Admin", null, new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), "ACTIVE", null, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("15f8f9bc-e701-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("180c2784-e700-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("29e3709e-e6ff-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("4f492fa4-e6ff-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("6e39f240-e6ff-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("98d579ca-0685-11ed-b939-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("b289b3a4-e6ff-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("d1a18b54-e6ff-11ec-8fea-0242ac120002"));

            migrationBuilder.DeleteData(
                table: "Field",
                keyColumn: "Id",
                keyValue: new Guid("fc24cff0-e6fd-11ec-8fea-0242ac120002"));

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
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("21d77b9a-f792-11ec-b939-0242ac120002"));

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

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"));
        }
    }
}
