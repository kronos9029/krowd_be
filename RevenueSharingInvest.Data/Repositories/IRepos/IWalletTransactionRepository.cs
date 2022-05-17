using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IWalletTransactionRepository
    {
        //CREATE
        public Task<int> CreateWalletTransaction(WalletTransaction walletTransactionDTO);

        //READ
        public Task<List<WalletTransaction>> GetAllWalletTransactions();
        public Task<WalletTransaction> GetWalletTransactionById(Guid walletTransactionId);

        //UPDATE
        public Task<int> UpdateWalletTransaction(WalletTransaction walletTransactionDTO, Guid walletTransactionId);

        //DELETE
        public Task<int> DeleteWalletTransactionById(Guid walletTransactionId);
    }
}
