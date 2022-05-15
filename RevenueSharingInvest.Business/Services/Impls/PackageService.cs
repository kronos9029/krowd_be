using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IMapper _mapper;


        public PackageService(IPackageRepository packageRepository, IMapper mapper)
        {
            _packageRepository = packageRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreatePackage(PackageDTO packageDTO)
        {
            int result;
            try
            {
                Package dto = _mapper.Map<Package>(packageDTO);
                result = await _packageRepository.CreatePackage(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Package Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeletePackageById(Guid packageId)
        {
            int result;
            try
            {

                result = await _packageRepository.DeletePackageById(packageId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Package Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<PackageDTO>> GetAllPackages()
        {
            List<Package> packageList = await _packageRepository.GetAllPackages();
            List<PackageDTO> list = _mapper.Map<List<PackageDTO>>(packageList);
            return list;
        }

        //GET BY ID
        public async Task<PackageDTO> GetPackageById(Guid packageId)
        {
            PackageDTO result;
            try
            {

                Package dto = await _packageRepository.GetPackageById(packageId);
                result = _mapper.Map<PackageDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Package Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdatePackage(PackageDTO packageDTO, Guid packageId)
        {
            int result;
            try
            {
                Package dto = _mapper.Map<Package>(packageDTO);
                result = await _packageRepository.UpdatePackage(dto, packageId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Package Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
