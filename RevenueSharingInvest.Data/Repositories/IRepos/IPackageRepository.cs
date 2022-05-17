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
        public Task<int> CreatePackage(Package packageDTO);

        //READ
        public Task<List<Package>> GetAllPackages();
        public Task<Package> GetPackageById(Guid packageId);

        //UPDATE
        public Task<int> UpdatePackage(Package packageDTO, Guid packageId);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
    }
}
