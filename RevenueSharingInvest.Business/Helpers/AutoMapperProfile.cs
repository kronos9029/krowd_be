using AutoMapper;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AccountTransaction, AccountTransactionDTO>().ReverseMap();
            CreateMap<Area, AreaDTO>().ReverseMap();
            CreateMap<BusinessField, BusinessFieldDTO>().ReverseMap();
            CreateMap<RevenueSharingInvest.Data.Models.Entities.Business, BusinessDTO>().ReverseMap();            
            CreateMap<Field, FieldDTO>().ReverseMap();
            CreateMap<Investment, InvestmentDTO>().ReverseMap();
            CreateMap<Investor, InvestorDTO>().ReverseMap();
            CreateMap<InvestorType, InvestorTypeDTO>().ReverseMap();
            CreateMap<InvestorWallet, InvestorWalletDTO>().ReverseMap();
            CreateMap<Package, PackageDTO>().ReverseMap();
            CreateMap<PackageVoucher, PackageVoucherDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<PeriodRevenueHistory, PeriodRevenueHistoryDTO>().ReverseMap();
            CreateMap<PeriodRevenue, PeriodRevenueDTO>().ReverseMap();
            CreateMap<ProjectEntity, ProjectEntityDTO>().ReverseMap();
            CreateMap<Project, ProjectDTO>().ReverseMap();           
            CreateMap<ProjectWallet, ProjectWalletDTO>().ReverseMap();
            CreateMap<Risk, RiskDTO>().ReverseMap();
            CreateMap<RiskType, RiskTypeDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<Stage, StageDTO>().ReverseMap();
            CreateMap<SystemWallet, SystemWalletDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<VoucherItem, VoucherItemDTO>().ReverseMap();
            CreateMap<Voucher, VoucherDTO>().ReverseMap();
            CreateMap<WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<WalletType, WalletTypeDTO>().ReverseMap();
            ///
            CreateMap<ProjectDTO, ProjectDetailDTO>().ReverseMap();
            
            CreateMap<Project, GetProjectDTO>().ReverseMap();
            CreateMap<Project, CreateProjectDTO>().ReverseMap();
            CreateMap<Project, UpdateProjectDTO>().ReverseMap();
            CreateMap<Project, InvestedProjectDTO>().ReverseMap();
            CreateMap<Project, BasicProjectDTO>().ReverseMap();
            CreateMap<GetProjectDTO, BasicProjectDTO>().ReverseMap();
           
            CreateMap<Data.Models.Entities.Business, GetBusinessDTO>().ReverseMap();
            CreateMap<Data.Models.Entities.Business, CreateBusinessDTO>().ReverseMap();
            CreateMap<Data.Models.Entities.Business, UpdateBusinessDTO>().ReverseMap();
            
            CreateMap<User, GetUserDTO>().ReverseMap();
            CreateMap<User, BusinessManagerUserDTO>().ReverseMap();
            CreateMap<User, ProjectManagerUserDTO>().ReverseMap();
            CreateMap<User, ProjectMemberUserDTO>().ForMember(des => des.investDate, act => act.MapFrom(src => src.CreateDate)).ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            
            CreateMap<ProjectEntity, GetProjectEntityDTO>().ReverseMap();
            CreateMap<ProjectEntity, ProjectComponentProjectEntityDTO>().ReverseMap();
            CreateMap<ProjectEntity, CreateProjectEntityDTO>().ReverseMap();
            CreateMap<ProjectEntity, UpdateProjectEntityDTO>().ReverseMap();

            CreateMap<Package, GetPackageDTO>().ReverseMap();
            CreateMap<Package, CreatePackageDTO>().ReverseMap();
            CreateMap<Package, UpdatePackageDTO>().ReverseMap();

            CreateMap<Investor, GetInvestorDTO>().ReverseMap();

            CreateMap<InvestorType, UserInvestorTypeDTO>().ReverseMap();

            CreateMap<Stage, CreateStageDTO>().ReverseMap();
            CreateMap<Stage, UpdateStageDTO>().ReverseMap();
            CreateMap<Stage, GetStageDTO>().ReverseMap();

            CreateMap<PeriodRevenue, CreatePeriodRevenueDTO>().ReverseMap();
            CreateMap<PeriodRevenue, UpdatePeriodRevenueDTO>().ReverseMap();
            CreateMap<PeriodRevenue, GetPeriodRevenueDTO>().ReverseMap();

            CreateMap<WalletType, GetWalletTypeDTO>().ReverseMap();
            CreateMap<WalletType, GetWalletTypeForWalletDTO>().ReverseMap();

            CreateMap<InvestorWallet, MappedInvestorWalletDTO>().ReverseMap();
            CreateMap<MappedInvestorWalletDTO, GetInvestorWalletDTO>().ReverseMap();

            CreateMap<ProjectWallet, MappedProjectWalletDTO>().ReverseMap();
            CreateMap<MappedProjectWalletDTO, GetProjectWalletDTO>().ReverseMap();

            CreateMap<AccountTransaction, MomoPaymentResult>().ReverseMap();

            CreateMap<Area, CreateUpdateAreaDTO>().ReverseMap();
            CreateMap<Area, GetAreaDTO>().ReverseMap();

            CreateMap<Investment, CreateInvestmentDTO>().ReverseMap();
            CreateMap<Investment, GetInvestmentDTO>().ReverseMap();

            CreateMap<Payment, InvestmentPaymentDTO>().ReverseMap();
            CreateMap<Payment, PeriodRevenuePaymentDTO>().ReverseMap();
        }
    }
}
