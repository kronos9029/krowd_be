using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IInvestorWalletService
    {
        //CREATE
        //public Task<IdDTO> CreateInvestorWallet(InvestorWalletDTO investorWalletDTO);

        //READ
        public Task<UserWalletsDTO> GetAllInvestorWallets(ThisUserObj currentUser);
        //public Task<InvestorWalletDTO> GetInvestorWalletById(Guid investorWalletId);

        //UPDATE
        //public Task<int> UpdateInvestorWallet(InvestorWalletDTO investorWalletDTO, Guid investorWalletId);

        //DELETE
        //public Task<int> DeleteInvestorWalletById(Guid investorWalletId);
        //public Task<int> ClearAllInvestorWalletData();
    }
}
