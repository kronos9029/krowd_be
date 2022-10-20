using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
        private readonly IProjectRepository _projectRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PackageService(IPackageRepository packageRepository, IProjectRepository projectRepository, IValidationService validationService, IMapper mapper)
        {
            _packageRepository = packageRepository;
            _projectRepository = projectRepository;
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreatePackage(CreatePackageDTO packageDTO, ThisUserObj currentUser)
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

                //Kiểm tra projectId có thuộc về business của PM không
                Project project = await _projectRepository.GetProjectById(Guid.Parse(packageDTO.projectId));
                if (!project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }    
                //

                if (packageDTO.price <= 0)
                    throw new InvalidFieldException("price must be greater than 0!!!");

                if (packageDTO.image != null && (packageDTO.image.Equals("string") || packageDTO.image.Length == 0))
                    packageDTO.image = null;

                if (packageDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (packageDTO.description == null || packageDTO.description.Trim().Length == 0)
                    throw new InvalidFieldException("description can not be null!!!");

                Package entity = _mapper.Map<Package>(packageDTO);

                entity.RemainingQuantity = entity.Quantity;
                entity.Status = PackageStatusEnum.INACTIVE.ToString(); //INACTIVE
                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                newId.id = await _packageRepository.CreatePackage(entity);

                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Package Object!");
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeletePackageById(Guid packageId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckUUIDFormat(packageId.ToString()))
                    throw new InvalidFieldException("Invalid packageId!!!");

                Package package = await _packageRepository.GetPackageById(packageId);
                if (package == null)
                    throw new NotFoundException("No Package Object Found!");

                result = await _packageRepository.DeletePackageById(packageId);
                if (result == 0)
                    throw new DeleteObjectException("Can Not Delete Package Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<AllProjectPackageDTO> GetAllPackagesByProjectId(Guid projectId, ThisUserObj currentUser)
        {
            try
            {
                if (projectId == null || !await _validationService.CheckUUIDFormat(projectId.ToString()))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", projectId))
                    throw new NotFoundException("This projectId is not existed!!!");

                //Kiểm tra projectId có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(projectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))) 
                    && !project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }
                //

                AllProjectPackageDTO result = new AllProjectPackageDTO();
                result.listOfPackage = new List<GetPackageDTO>();

                result.numOfPackage = await _packageRepository.CountPackageByProjectId(projectId);

                List<Package> packageList = await _packageRepository.GetAllPackagesByProjectId(projectId);
                //List<GetPackageDTO> list = _mapper.Map<List<GetPackageDTO>>(packageList);
                GetPackageDTO dto = new GetPackageDTO();

                foreach (Package item in packageList)
                {
                    dto = _mapper.Map<GetPackageDTO>(item);
                    dto.descriptionList = new List<string>();

                    dto.createDate = await _validationService.FormatDateOutput(dto.createDate);
                    dto.updateDate = await _validationService.FormatDateOutput(dto.updateDate);

                    if (item.Description != null)
                    {
                        string[] split = item.Description.Split("\\li", StringSplitOptions.RemoveEmptyEntries);
                        foreach (string descriptionItem in split)
                        {
                            dto.descriptionList.Add(descriptionItem);
                        }
                    }
                    else
                        dto.descriptionList = null;

                    result.listOfPackage.Add(dto);
                }

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetPackageDTO> GetPackageById(Guid packageId, ThisUserObj currentUser)
        {
            try
            {
                Package package = await _packageRepository.GetPackageById(packageId);
                if (package == null)
                    throw new NotFoundException("No Package Object Found!");

                //Kiểm tra projectId của Package đó có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(package.ProjectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    && !project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This Package's projectId is not belong to your's Business!!!");
                }
                //

                GetPackageDTO dto = new GetPackageDTO();

                dto = _mapper.Map<GetPackageDTO>(package);
                dto.descriptionList = new List<string>();

                dto.createDate = await _validationService.FormatDateOutput(dto.createDate);
                dto.updateDate = await _validationService.FormatDateOutput(dto.updateDate);
                if (package.Description != null)
                {
                    string[] split = package.Description.Split("\\li", StringSplitOptions.RemoveEmptyEntries);
                    foreach (string descriptionItem in split)
                    {
                        dto.descriptionList.Add(descriptionItem);
                    }
                }
                else
                    dto.descriptionList = null;



                return dto;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackage(UpdatePackageDTO packageDTO, Guid packageId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                Package package = await _packageRepository.GetPackageById(packageId);
                if (package == null)
                    throw new InvalidFieldException("No Package Found!!!");

                Project project = await _projectRepository.GetProjectById(package.ProjectId);
                if (!project.ManagerId.ToString().Equals(currentUser.userId))
                {
                    throw new NotFoundException("This packageId is not belong to your Project!!!");
                }

                if (packageDTO.name != null && !await _validationService.CheckText(packageDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");                

                if (packageDTO.price != 0 && packageDTO.price <= 0)
                    throw new InvalidFieldException("price must be greater than 0!!!");

                if (packageDTO.quantity != 0 && packageDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                Package entity = _mapper.Map<Package>(packageDTO);

                if (packageDTO.price == 0)
                    entity.Price = package.Price;

                if (packageDTO.quantity == 0)
                    entity.Quantity = package.Quantity;

                if (packageDTO.quantity != 0)
                {
                    entity.RemainingQuantity = entity.Quantity;
                }                   
                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _packageRepository.UpdatePackage(entity, packageId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Package Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
