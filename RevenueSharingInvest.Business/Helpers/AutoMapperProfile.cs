using AutoMapper;
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
            CreateMap<Project, CreateUpdateProjectDTO>().ReverseMap();
           
            CreateMap<Data.Models.Entities.Business, GetBusinessDTO>().ReverseMap();
            CreateMap<Data.Models.Entities.Business, CreateUpdateBusinessDTO>().ReverseMap();
            
            CreateMap<User, GetUserDTO>().ReverseMap();
            CreateMap<User, BusinessManagerUserDTO>().ReverseMap();
            CreateMap<User, ProjectManagerUserDTO>().ReverseMap();
            CreateMap<User, ProjectMemberUserDTO>().ForMember(des => des.investDate, act => act.MapFrom(src => src.CreateDate)).ReverseMap();
            CreateMap<User, CreateUpdateUserDTO>().ReverseMap();
            
            CreateMap<ProjectEntity, GetProjectEntityDTO>().ReverseMap();
            CreateMap<ProjectEntity, ProjectComponentProjectEntityDTO>().ReverseMap();
            CreateMap<ProjectEntity, CreateUpdateProjectEntityDTO>().ReverseMap();

            CreateMap<Package, GetPackageDTO>().ReverseMap();
            CreateMap<Package, CreateUpdatePackageDTO>().ReverseMap();
        }
    }
}
