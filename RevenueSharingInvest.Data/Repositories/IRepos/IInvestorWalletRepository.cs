﻿using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<List<InvestorWallet>> GetInvestorWalletsByInvestorId(Guid investorId);
        public Task<InvestorWallet> GetInvestorWalletByInvestorIdAndType(Guid investorId, string walletType);
        public Task<InvestorWallet> GetInvestorWalletById(Guid id);
        public Task<double> GetInvestorWalletBalanceById(Guid id);
        public Task<InvestorWallet> GetInvestorWalletByUserIdAndWalletTypeId(Guid userId, Guid walletTypeId);
        public Task<string> GetInvertorWalletNamebyWalletId(Guid walletId);
        //UPDATE
        public Task<int> UpdateInvestorWallet(InvestorWallet investorWalletDTO, Guid investorWalletId);
        //public Task<int> InvestorTopUpWallet(InvestorWallet investorWalletDTO);
        public Task<int> UpdateInvestorWalletBalance(InvestorWallet investorWalletDTO);
        public Task<int> UpdateWalletBalance(dynamic investorWalletDTO);

        //DELETE
        public Task<int> DeleteInvestorWalletById(Guid investorWalletId);
    }
}
