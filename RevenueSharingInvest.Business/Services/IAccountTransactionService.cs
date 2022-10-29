using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<string> CreateTopUpAccountTransaction(MomoPaymentResult momoPaymentResult);

        //READ
        public Task<AllAccountTransactionDTO> GetAllAccountTransactions(int pageIndex, int pageSize, string sort, ThisUserObj currentUser);
        public Task<AllAccountTransactionDTO> GetAccountTransactionsByDate(int pageIndex, int pageSize, string fromDate, string toDate, string sort, ThisUserObj currentUser);
        public Task<string> CreateWithdrawAccountTransaction(dynamic wallet, WithdrawRequest withdrawRequest, string userId, string roleName);
        //public Task<AccountTransactionDTO> GetAccountTransactionById(Guid accountTransactionId);

        //UPDATE
        //public Task<int> UpdateAccountTransaction(AccountTransactionDTO accountTransactionDTO, Guid accountTransactionId);

        //DELETE
        //public Task<int> DeleteAccountTransactionById(Guid accountTransactionId);
        //public Task<int> ClearAllAccountTransactionData();
    }
}
