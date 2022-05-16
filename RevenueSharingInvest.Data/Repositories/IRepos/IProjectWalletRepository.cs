using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IProjectWalletRepository
    {
        //CREATE
        public Task<int> CreateProjectWallet(ProjectWallet projectWalletDTO);

        //READ
        public Task<List<ProjectWallet>> GetAllProjectWallets();
        public Task<ProjectWallet> GetProjectWalletById(Guid projectWalletId);

        //UPDATE
        public Task<int> UpdateProjectWallet(ProjectWallet projectWalletDTO, Guid projectWalletId);

        //DELETE
        public Task<int> DeleteProjectWalletById(Guid projectWalletId);
    }
}
