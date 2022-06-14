using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPackageService
    {
        //CREATE
        public Task<IdDTO> CreatePackage(PackageDTO packageDTO);

        //READ
        public Task<List<PackageDTO>> GetAllPackages(int pageIndex, int pageSize);
        public Task<PackageDTO> GetPackageById(Guid packageId);

        //UPDATE
        public Task<int> UpdatePackage(PackageDTO packageDTO, Guid packageId);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
        public Task<int> ClearAllPackageData();
    }
}
