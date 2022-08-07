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
        public Task<IdDTO> CreatePackage(CreateUpdatePackageDTO packageDTO);

        //READ
        public Task<AllProjectPackageDTO> GetAllPackagesByProjectId(int pageIndex, int pageSize, string projectId);
        public Task<GetPackageDTO> GetPackageById(Guid packageId);

        //UPDATE
        public Task<int> UpdatePackage(CreateUpdatePackageDTO packageDTO, Guid packageId);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
        public Task<int> ClearAllPackageData();
    }
}
