using RevenueSharingInvest.Data.Models.DTOs;
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
        public Task<int> CreateInvestorWallet(InvestorWalletDTO investorWalletDTO);

        //READ
        public Task<List<InvestorWalletDTO>> GetAllInvestorWallets();
        public Task<InvestorWalletDTO> GetInvestorWalletById(Guid investorWalletId);

        //UPDATE
        public Task<int> UpdateInvestorWallet(InvestorWalletDTO investorWalletDTO, Guid investorWalletId);

        //DELETE
        public Task<int> DeleteInvestorWalletById(Guid investorWalletId);
    }
}
