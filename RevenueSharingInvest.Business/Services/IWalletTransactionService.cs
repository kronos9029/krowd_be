using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<IdDTO> CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO);
        public Task<string> TransferFromI1ToI2(ThisUserObj currentUser, double amount);

        //READ
        public Task<List<WalletTransactionDTO>> GetAllWalletTransactions(int pageIndex, int pageSize);
        public Task<WalletTransactionDTO> GetWalletTransactionById(Guid walletTransactionId);

        //UPDATE
        public Task<int> UpdateWalletTransaction(WalletTransactionDTO walletTransactionDTO, Guid walletTransactionId);

        //DELETE
        public Task<int> DeleteWalletTransactionById(Guid walletTransactionId);
        public Task<int> ClearAllWalletTransactionData();
    }
}
