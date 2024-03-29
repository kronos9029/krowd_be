﻿using RevenueSharingInvest.API;
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
        public Task<IdDTO> CreatePackage(CreatePackageDTO packageDTO, ThisUserObj currentUser);

        //READ
        public Task<AllProjectPackageDTO> GetAllPackagesByProjectId(Guid projectId, ThisUserObj currentUser);
        public Task<GetPackageDTO> GetPackageById(Guid packageId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdatePackage(UpdatePackageDTO packageDTO, Guid packageId, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeletePackageById(Guid packageId);
        //public Task<int> ClearAllPackageData();
    }
}
