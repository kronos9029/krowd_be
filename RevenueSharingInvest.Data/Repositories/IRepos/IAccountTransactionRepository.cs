using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IAccountTransactionRepository
    {
        //CREATE
        public Task<int> CreateAccountTransaction(AccountTransaction accountTransactionDTO);

        //READ
        public Task<List<AccountTransaction>> GetAllAccountTransactions();
        public Task<AccountTransaction> GetAccountTransactionById(Guid accountTransactionId);

        //UPDATE
        public Task<int> UpdateAccountTransaction(AccountTransaction accountTransactionDTO, Guid accountTransactionId);

        //DELETE
        public Task<int> DeleteAccountTransactionById(Guid accountTransactionId);
    }
}
