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
        public Task<string> CreateProjectWallet(ProjectWallet projectWalletDTO);

        //READ
        public Task<List<ProjectWallet>> GetProjectWalletsByProjectManagerId(Guid projectManagerId);
        public Task<ProjectWallet> GetProjectWalletById(Guid id);
        public Task<ProjectWallet> GetProjectWalletByProjectManagerIdAndType(Guid projectOwnerId, string walletType, Guid? projectId);

        //UPDATE
        public Task<int> UpdateProjectWallet(ProjectWallet projectWalletDTO, Guid projectWalletId);
        public Task<int> UpdateProjectWalletBalance(ProjectWallet projectWalletDTO);
        public Task<int> UpdateProjectWalletBalanceToActivate(ProjectWallet projectWalletDTO);

        //DELETE
        public Task<int> DeleteProjectWalletByBusinessId(Guid businessId);
    }
}
