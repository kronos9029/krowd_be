using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IWalletTransactionService
    {
        //CREATE
        public Task<int> CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO);

        //READ
        public Task<List<WalletTransactionDTO>> GetAllWalletTransactions();
        public Task<WalletTransactionDTO> GetWalletTransactionById(Guid walletTransactionId);

        //UPDATE
        public Task<int> UpdateWalletTransaction(WalletTransactionDTO walletTransactionDTO, Guid walletTransactionId);

        //DELETE
        public Task<int> DeleteWalletTransactionById(Guid walletTransactionId);
    }
}
