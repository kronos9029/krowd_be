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

        //READ
        public Task<List<WalletTransactionDTO>> GetAllWalletTransactions(int pageIndex, int pageSize);
        public Task<WalletTransactionDTO> GetWalletTransactionById(Guid walletTransactionId);

        //UPDATE

        //DELETE
    }
}
