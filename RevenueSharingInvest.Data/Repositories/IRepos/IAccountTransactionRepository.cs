﻿using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<string> CreateAccountTransaction(AccountTransaction accountTransactionDTO);

        //READ
        public Task<List<AccountTransaction>> GetAllAccountTransactions(int pageIndex, int pageSize);
        public Task<AccountTransaction> GetAccountTransactionById(Guid accountTransactionId);

        //UPDATE
        public Task<int> UpdateAccountTransaction(AccountTransaction accountTransactionDTO, Guid accountTransactionId);

        //DELETE
        public Task<int> DeleteAccountTransactionById(Guid accountTransactionId);
        public Task<int> ClearAllAccountTransactionData();
    }
}
