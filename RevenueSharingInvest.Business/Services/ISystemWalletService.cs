using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface ISystemWalletService
    {
        //CREATE
        public Task<int> CreateSystemWallet(SystemWalletDTO systemWalletDTO);

        //READ
        public Task<List<SystemWalletDTO>> GetAllSystemWallets();
        public Task<SystemWalletDTO> GetSystemWalletById(Guid systemWalletId);

        //UPDATE
        public Task<int> UpdateSystemWallet(SystemWalletDTO systemWalletDTO, Guid systemWalletId);

        //DELETE
        public Task<int> DeleteSystemWalletById(Guid systemWalletId);
    }
}
