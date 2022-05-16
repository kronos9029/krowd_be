using RevenueSharingInvest.Data.Models.DTOs;
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
        public Task<int> CreateProjectWallet(ProjectWalletDTO projectWalletDTO);

        //READ
        public Task<List<ProjectWalletDTO>> GetAllProjectWallets();
        public Task<ProjectWalletDTO> GetProjectWalletById(Guid projectWalletId);

        //UPDATE
        public Task<int> UpdateProjectWallet(ProjectWalletDTO projectWalletDTO, Guid projectWalletId);

        //DELETE
        public Task<int> DeleteProjectWalletById(Guid projectWalletId);
    }
}
