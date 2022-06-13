using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IWalletTypeRepository
    {
        //CREATE
        public Task<string> CreateWalletType(WalletType walletTypeDTO);

        //READ
        public Task<List<WalletType>> GetAllWalletTypes();
        public Task<WalletType> GetWalletTypeById(Guid walletTypeId);

        //UPDATE
        public Task<int> UpdateWalletType(WalletType walletTypeDTO, Guid walletTypeId);

        //DELETE
        public Task<int> DeleteWalletTypeById(Guid walletTypeId);
        public Task<int> ClearAllWalletTypeData();
    }
}
