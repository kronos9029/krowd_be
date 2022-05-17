using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IAccountTransactionService
    {
        //CREATE
        public Task<int> CreateAccountTransaction(AccountTransactionDTO accountTransactionDTO);

        //READ
        public Task<List<AccountTransactionDTO>> GetAllAccountTransactions();
        public Task<AccountTransactionDTO> GetAccountTransactionById(Guid accountTransactionId);

        //UPDATE
        public Task<int> UpdateAccountTransaction(AccountTransactionDTO accountTransactionDTO, Guid accountTransactionId);

        //DELETE
        public Task<int> DeleteAccountTransactionById(Guid accountTransactionId);
    }
}
