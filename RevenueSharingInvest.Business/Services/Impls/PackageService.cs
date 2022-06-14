using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PackageService(IPackageRepository packageRepository, IValidationService validationService, IMapper mapper)
        {
            _packageRepository = packageRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllPackageData()
        {
            int result;
            try
            {
                result = await _packageRepository.ClearAllPackageData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreatePackage(PackageDTO packageDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(packageDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (packageDTO.projectId == null || !await _validationService.CheckUUIDFormat(packageDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(packageDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (packageDTO.price <= 0)
                    throw new InvalidFieldException("price must be greater than 0!!!");

                if (packageDTO.image != null && (packageDTO.image.Equals("string") || packageDTO.image.Length == 0))
                    packageDTO.image = null;

                if (packageDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (packageDTO.description != null && (packageDTO.description.Equals("string") || packageDTO.description.Length == 0))
                    packageDTO.description = null;

                if (packageDTO.minForPurchasing <= 0)
                    throw new InvalidFieldException("minForPurchasing must be greater than 0!!!");

                if (packageDTO.maxForPurchasing <= 0)
                    throw new InvalidFieldException("maxForPurchasing must be greater than 0!!!");

                if (!await _validationService.CheckDate((packageDTO.openDate)))
                    throw new InvalidFieldException("Invalid openDate!!!");

                if (!await _validationService.CheckDate((packageDTO.closeDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                packageDTO.approvedBy = null;

                if (packageDTO.createBy != null && packageDTO.createBy.Length >= 0)
                {
                    if (packageDTO.createBy.Equals("string"))
                        packageDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (packageDTO.updateBy != null && packageDTO.updateBy.Length >= 0)
                {
                    if (packageDTO.updateBy.Equals("string"))
                        packageDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                packageDTO.isDeleted = false;

                Package dto = _mapper.Map<Package>(packageDTO);
                newId.id = await _packageRepository.CreatePackage(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Package Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete Package Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PackageDTO>> GetAllPackages(int pageIndex, int pageSize)
        {
            try
            {
                List<Package> packageList = await _packageRepository.GetAllPackages(pageIndex, pageSize);
                List<PackageDTO> list = _mapper.Map<List<PackageDTO>>(packageList);
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No Package Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackage(PackageDTO packageDTO, Guid packageId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(packageDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (packageDTO.projectId == null || !await _validationService.CheckUUIDFormat(packageDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(packageDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (packageDTO.price <= 0)
                    throw new InvalidFieldException("price must be greater than 0!!!");

                if (packageDTO.image != null && (packageDTO.image.Equals("string") || packageDTO.image.Length == 0))
                    packageDTO.image = null;

                if (packageDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (packageDTO.description != null && (packageDTO.description.Equals("string") || packageDTO.description.Length == 0))
                    packageDTO.description = null;

                if (packageDTO.minForPurchasing <= 0)
                    throw new InvalidFieldException("minForPurchasing must be greater than 0!!!");

                if (packageDTO.maxForPurchasing <= 0)
                    throw new InvalidFieldException("maxForPurchasing must be greater than 0!!!");

                if (!await _validationService.CheckDate((packageDTO.openDate)))
                    throw new InvalidFieldException("Invalid openDate!!!");

                if (!await _validationService.CheckDate((packageDTO.closeDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                packageDTO.approvedBy = null;

                if (packageDTO.createBy != null && packageDTO.createBy.Length >= 0)
                {
                    if (packageDTO.createBy.Equals("string"))
                        packageDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (packageDTO.updateBy != null && packageDTO.updateBy.Length >= 0)
                {
                    if (packageDTO.updateBy.Equals("string"))
                        packageDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Package dto = _mapper.Map<Package>(packageDTO);
                result = await _packageRepository.UpdatePackage(dto, packageId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Package Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
