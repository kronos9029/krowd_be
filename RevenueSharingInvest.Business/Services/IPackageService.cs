using RevenueSharingInvest.API;
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
        public Task<IdDTO> CreatePackage(CreateUpdatePackageDTO packageDTO, ThisUserObj currentUser);

        //READ
        public Task<AllProjectPackageDTO> GetAllPackagesByProjectId(string projectId, ThisUserObj currentUser);
        public Task<GetPackageDTO> GetPackageById(Guid packageId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdatePackage(CreateUpdatePackageDTO packageDTO, Guid packageId, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
        //public Task<int> ClearAllPackageData();
    }
}
