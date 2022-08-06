using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IPackageRepository
    {
        //CREATE
        public Task<string> CreatePackage(Package packageDTO);

        //READ
        public Task<List<Package>> GetAllPackagesByProjectId(int pageIndex, int pageSize, Guid projectId);
        public Task<Package> GetPackageById(Guid packageId);
        public Task<int> CountPackageByProjectId(Guid projectId);

        //UPDATE
        public Task<int> UpdatePackage(Package packageDTO, Guid packageId);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
        public Task<int> ClearAllPackageData();
    }
}
