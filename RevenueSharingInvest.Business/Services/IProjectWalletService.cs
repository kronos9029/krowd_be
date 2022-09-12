using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IProjectWalletService
    {
        //CREATE
        //public Task<IdDTO> CreateProjectWallet(ProjectWalletDTO projectWalletDTO);

        //READ
        public Task<UserWalletsDTO> GetAllProjectWallets(ThisUserObj currentUser);
        //public Task<ProjectWalletDTO> GetProjectWalletById(Guid projectWalletId);

        //UPDATE
        //public Task<int> UpdateProjectWallet(ProjectWalletDTO projectWalletDTO, Guid projectWalletId);

        //DELETE
        //public Task<int> DeleteProjectWalletById(Guid projectWalletId);
        //public Task<int> ClearAllProjectWalletData();
    }
}
