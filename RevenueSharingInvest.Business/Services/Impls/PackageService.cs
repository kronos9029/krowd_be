using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Data.Models.Constants;
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
        public async Task<IdDTO> CreatePackage(CreateUpdatePackageDTO packageDTO)
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

                //if (packageDTO.createBy != null && packageDTO.createBy.Length >= 0)
                //{
                //    if (packageDTO.createBy.Equals("string"))
                //        packageDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(packageDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (packageDTO.updateBy != null && packageDTO.updateBy.Length >= 0)
                //{
                //    if (packageDTO.updateBy.Equals("string"))
                //        packageDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(packageDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                Package entity = _mapper.Map<Package>(packageDTO);

                entity.RemainingQuantity = entity.Quantity;

                entity.Status = Enum.GetNames(typeof(PackageStatusEnum)).ElementAt(0);

                newId.id = await _packageRepository.CreatePackage(entity);
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
        public async Task<AllProjectPackageDTO> GetAllPackagesByProjectId(int pageIndex, int pageSize, string projectId)
        {
            try
            {
                if (projectId == null || !await _validationService.CheckUUIDFormat(projectId.ToString()))
                    throw new InvalidFieldException("Invalid projectId!!!");

                AllProjectPackageDTO result = new AllProjectPackageDTO();
                result.listOfPackage = new List<GetPackageDTO>();

                result.numOfPackage = await _packageRepository.CountPackageByProjectId(Guid.Parse(projectId));

                List<Package> packageList = await _packageRepository.GetAllPackagesByProjectId(pageIndex, pageSize, Guid.Parse(projectId));
                List<GetPackageDTO> list = _mapper.Map<List<GetPackageDTO>>(packageList);

                foreach (GetPackageDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    result.listOfPackage.Add(item);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetPackageDTO> GetPackageById(Guid packageId)
        {
            try
            {
                if (packageId == null || !await _validationService.CheckUUIDFormat(packageId.ToString()))
                    throw new InvalidFieldException("Invalid packageId!!!");

                Package package = await _packageRepository.GetPackageById(packageId);
                GetPackageDTO packageDTO = _mapper.Map<GetPackageDTO>(package);                              
                if (packageDTO == null)
                    throw new NotFoundException("No Package Object Found!");

                packageDTO.createDate = await _validationService.FormatDateOutput(packageDTO.createDate);
                packageDTO.updateDate = await _validationService.FormatDateOutput(packageDTO.updateDate);

                return packageDTO;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackage(CreateUpdatePackageDTO packageDTO, Guid packageId)
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

                //if (packageDTO.createBy != null && packageDTO.createBy.Length >= 0)
                //{
                //    if (packageDTO.createBy.Equals("string"))
                //        packageDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(packageDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (packageDTO.updateBy != null && packageDTO.updateBy.Length >= 0)
                //{
                //    if (packageDTO.updateBy.Equals("string"))
                //        packageDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(packageDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                Package entity = _mapper.Map<Package>(packageDTO);

                entity.RemainingQuantity = entity.Quantity;

                result = await _packageRepository.UpdatePackage(entity, packageId);
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
