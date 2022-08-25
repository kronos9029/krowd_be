using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestorWalletRepository
    {
        //CREATE
        public Task<int> CreateInvestorWallet(Guid investorId, Guid walletTypeId, Guid currentUserId);

        //READ
        public Task<List<InvestorWallet>> GetAllInvestorWallets(int pageIndex, int pageSize);
        public Task<InvestorWallet> GetInvestorWalletById(Guid investorWalletId);

        //UPDATE
        public Task<int> UpdateInvestorWallet(InvestorWallet investorWalletDTO, Guid investorWalletId);

        //DELETE
        public Task<int> DeleteInvestorWalletById(Guid investorWalletId);
        public Task<int> ClearAllInvestorWalletData();
    }
}
