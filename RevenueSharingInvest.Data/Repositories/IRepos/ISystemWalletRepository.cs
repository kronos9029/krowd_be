﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface ISystemWalletRepository
    {
        //CREATE
        public Task<string> CreateSystemWallet(SystemWallet systemWalletDTO);

        //READ
        public Task<List<SystemWallet>> GetAllSystemWallets(int pageIndex, int pageSize);
        public Task<SystemWallet> GetSystemWalletById(Guid systemWalletId);

        //UPDATE
        public Task<int> UpdateSystemWallet(SystemWallet systemWalletDTO, Guid systemWalletId);

        //DELETE
        public Task<int> DeleteSystemWalletById(Guid systemWalletId);
        public Task<int> ClearAllSystemWalletData();
    }
}
