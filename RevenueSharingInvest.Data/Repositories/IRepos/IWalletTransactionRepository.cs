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
        public Task<string> CreateWalletTransaction(WalletTransaction walletTransactionDTO);

        //READ
        public Task<List<WalletTransaction>> GetAllWalletTransactions(int pageIndex, int pageSize, Guid? userId, Guid? userRoleId, Guid? walletId, string fromDate, string toDate, string type, string order);
        public Task<WalletTransaction> GetWalletTransactionById(Guid walletTransactionId);

        //UPDATE

        //DELETE
    }
}
