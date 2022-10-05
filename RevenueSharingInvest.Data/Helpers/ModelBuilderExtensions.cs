using Microsoft.EntityFrameworkCore;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Helpers
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            DateTime localDate = DateTime.Now;

            modelBuilder.Entity<Role>().HasData( 
                new Role { Id = Guid.Parse("015AE3C5-EEE9-4F5C-BEFB-57D41A43D9DF"), Name = RoleEnum.BUSINESS_MANAGER.ToString(), Description = "Business manager", CreateDate = localDate },
                new Role { Id = Guid.Parse("AD5F37DA-CA48-4DC5-9F4B-963D94B535E6"), Name = RoleEnum.INVESTOR.ToString(), Description = "Investor", CreateDate = localDate },
                new Role { Id = Guid.Parse("2D80393A-3A3D-495D-8DD7-F9261F85CC8F"), Name = RoleEnum.PROJECT_OWNER.ToString(), Description = "Project owner", CreateDate = localDate },
                new Role { Id = Guid.Parse("FF54ACC6-C4E9-4B73-A158-FD640B4B6940"), Name = RoleEnum.ADMIN.ToString(), Description = "Krowd's admin", CreateDate = localDate }
                );            
            
            modelBuilder.Entity<InvestorType>().HasData( 
                new InvestorType { Id = Guid.Parse("EC92EF2A-F794-11EC-B939-0242AC120002"), Name = InvestorTypeEnum.LONG_INVESTOR.ToString(), Description = "Mua và nắm giữ dài hạn, thường là các HOLDER, họ có niềm tin vào tiền điện tử cũng như công nghệ Blockchain.", CreateDate = localDate },
                new InvestorType { Id = Guid.Parse("07C55F72-F794-11EC-B939-0242AC120002"), Name = InvestorTypeEnum.HOBBYIST_INVESTOR.ToString(), Description = "Những người trẻ tuổi, quan tâm đến những thứ mới lạ có thể mang lại sự thay đổi cho tương lai và đặc biệt thích về các công nghệ.", CreateDate = localDate },
                new InvestorType { Id = Guid.Parse("CA4E68CC-F794-11EC-B939-0242AC120002"), Name = InvestorTypeEnum.SHORT_INVESTOR.ToString(), Description = "Những người đầu tư ngắn hạn, thường là Day Trader, họ tìm kiếm lợi nhuận từ những biến động của thị trường trong thời gian ngắn, thường sử dụng margin để gia tăng lợi nhuận.", CreateDate = localDate },
                new InvestorType { Id = Guid.Parse("175389B8-F795-11EC-B939-0242AC120002"), Name = InvestorTypeEnum.STRATEGIC_INVESTOR.ToString(), Description = "Họ thường là những người trẻ tuổi, có kiến thức tốt và thấu hiểu bản thân, nhìn thì trường bằng con mắt đa chiều và cũng rất thông minh.", CreateDate = localDate }
                );

            modelBuilder.Entity<WalletType>().HasData(
                new WalletType { Id = Guid.Parse("A036A7D2-980B-41B2-8EC2-06BFF8782B66"), Name = WalletTypeEnum.PROJECT_INVESTMENT_WALLET.ToString(), Description = "Ví đầu tư dự án của Business", Mode = ModeEnum.BUSINESS.ToString(), Type = TypeEnum.B3.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("0568667C-1E13-440B-8D4A-077288AA9919"), Name = WalletTypeEnum.UNIVERSAL_PAYMENT_WALLET.ToString(), Description = "Ví thanh toán chung của Business", Mode = ModeEnum.BUSINESS.ToString(), Type = TypeEnum.B1.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("67453687-E268-4F32-8FB8-0E7C77DE2C71"), Name = WalletTypeEnum.TEMPORARY_WALLET.ToString(), Description = "Ví tạm thời của Investor", Mode = ModeEnum.INVESTOR.ToString(), Type = TypeEnum.I1.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("E3B41A08-135B-4FB3-BF1F-4D3675D39F96"), Name = WalletTypeEnum.GENERAL_INVESTMENT_WALLET.ToString(), Description = "Ví đầu tư chung của Investors", Mode = ModeEnum.INVESTOR.ToString(), Type = TypeEnum.I2.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("C485DC8B-B61D-4DE9-8939-4E765A3F9E7D"), Name = WalletTypeEnum.INVESTOR_COLLECTING_WALLET.ToString(), Description = "Ví thu tiền của Investor", Mode = ModeEnum.INVESTOR.ToString(), Type = TypeEnum.I5.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("BA9BAF2F-B063-41A2-808B-A452AFA3E57F"), Name = WalletTypeEnum.ADVANCE_WALLET.ToString(), Description = "Ví tạm ứng của Investor", Mode = ModeEnum.INVESTOR.ToString(), Type = TypeEnum.I3.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("4E24A3D5-9AED-4DB2-87F5-BD69C55899B7"), Name = WalletTypeEnum.PROJECT_PAYMENT_WALLET.ToString(), Description = "Ví thanh toán dự án của Investor", Mode = ModeEnum.INVESTOR.ToString(), Type = TypeEnum.I4.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("05D47EB3-06A5-4718-A46A-D62494DEE371"), Name = WalletTypeEnum.BUSINESS_PAYMENT_WALLET.ToString(), Description = "Ví thanh toán doanh nghiệp của Business", Mode = ModeEnum.BUSINESS.ToString(), Type = TypeEnum.B2.ToString(), CreateDate = localDate },
                new WalletType { Id = Guid.Parse("D7ED0979-285F-4EC0-9F6B-AE95FCFA9207"), Name = WalletTypeEnum.BUSINESS_COLLECTING_WALLET.ToString(), Description = "Ví thu tiền của Business", Mode = ModeEnum.BUSINESS.ToString(), Type = TypeEnum.B4.ToString(), CreateDate = localDate }
                );

            modelBuilder.Entity<User>().HasData(
                new User { Id = Guid.Parse("21D77B9A-F792-11EC-B939-0242AC120002"), RoleId = Guid.Parse("FF54ACC6-C4E9-4B73-A158-FD640B4B6940"), Description = "Admin 1", LastName = "Admin", FirstName = "Krowd's", Image = "https://firebasestorage.googleapis.com/v0/b/revenuesharinginvest-44354.appspot.com/o/User%2Favt%20Kh%C3%A1nh.jpg?alt=media&token=0940aab1-edaf-443b-ad83-1d14cb8dff1f", Email = "krowd.dev.2022@gmail.com", Gender="LGBT", Status = "ACTIVE", CreateDate = localDate }
                );

            modelBuilder.Entity<Field>().HasData(
                new Field { Id = Guid.Parse("180c2784-e700-11ec-8fea-0242ac120002"), Name= "Education",Description= "School, Tutor, Learning tools", CreateDate = localDate},
                new Field { Id = Guid.Parse("15f8f9bc-e701-11ec-8fea-0242ac120002"), Name= "Beauty", Description= "Spa, Cosmetic, Hair salon", CreateDate = localDate},
                new Field { Id = Guid.Parse("fc24cff0-e6fd-11ec-8fea-0242ac120002"), Name= "Food", Description= "Restaurant, Food Court, Culinary", CreateDate = localDate},
                new Field { Id = Guid.Parse("6e39f240-e6ff-11ec-8fea-0242ac120002"), Name= "Health", Description= "Functional foods, Clean food", CreateDate = localDate},
                new Field { Id = Guid.Parse("d1a18b54-e6ff-11ec-8fea-0242ac120002"), Name= "Fitness", Description= "Gym, Sportwear, Exercise machines", CreateDate = localDate},
                new Field { Id = Guid.Parse("29e3709e-e6ff-11ec-8fea-0242ac120002"), Name= "Drink", Description= "Restaurant, Drink Court, Culinary, Cafe", CreateDate = localDate},
                new Field { Id = Guid.Parse("4f492fa4-e6ff-11ec-8fea-0242ac120002"), Name= "Fashion", Description= "Clothes, Shoes, Bags", CreateDate = localDate},
                new Field { Id = Guid.Parse("b289b3a4-e6ff-11ec-8fea-0242ac120002"), Name= "Medical", Description= "Drug, Medical devices", CreateDate = localDate},
                new Field { Id = Guid.Parse("98d579ca-0685-11ed-b939-0242ac120002"), Name= "Grocery", Description= "Food, Drink, Personal belongings", CreateDate = localDate}
                );


        }
        public enum InvestorTypeEnum
        {
            LONG_INVESTOR,
            HOBBYIST_INVESTOR,
            SHORT_INVESTOR,
            STRATEGIC_INVESTOR
        }
        public enum WalletTypeEnum
        {
            PROJECT_INVESTMENT_WALLET,
            UNIVERSAL_PAYMENT_WALLET,
            TEMPORARY_WALLET,
            GENERAL_INVESTMENT_WALLET,
            INVESTOR_COLLECTING_WALLET,
            BUSINESS_COLLECTING_WALLET,
            ADVANCE_WALLET,
            PROJECT_PAYMENT_WALLET,
            BUSINESS_PAYMENT_WALLET
        }
        public enum ModeEnum
        {
            BUSINESS,
            INVESTOR
        }
        public enum TypeEnum
        {
            B1,
            B2,
            B3,
            B4,
            I1,
            I2,
            I3,
            I4,
            I5,
        }
    }
}
