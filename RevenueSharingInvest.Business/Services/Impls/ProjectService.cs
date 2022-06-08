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
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_PROJECT_OWNER_ID = "2d80393a-3a3d-495d-8dd7-f9261f85cc8f";

        public ProjectService(IProjectRepository projectRepository, IValidationService validationService, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllProjectData()
        {
            int result;
            try
            {
                result = await _projectRepository.ClearAllProjectData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateProject(ProjectDTO projectDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (projectDTO.managerId == null || !await _validationService.CheckUUIDFormat(projectDTO.managerId))
                    throw new InvalidFieldException("Invalid managerId!!!");

                if (!await _validationService.CheckExistenceUserWithRole(ROLE_PROJECT_OWNER_ID, Guid.Parse(projectDTO.managerId)))
                    throw new NotFoundException("This ManagerId is not existed!!!");

                if (projectDTO.businessId == null || !await _validationService.CheckUUIDFormat(projectDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(projectDTO.businessId)))
                    throw new NotFoundException("This BusinessId is not existed!!!");

                //if (projectDTO.areaId == null || !await _validationService.CheckUUIDFormat(projectDTO.areaId))
                //    throw new InvalidFieldException("Invalid areaId!!!");

                //if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                //    throw new NotFoundException("This areaId is not existed!!!");

                if (!await _validationService.CheckText(projectDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (projectDTO.image != null && (projectDTO.image.Equals("string") || projectDTO.image.Length == 0))
                    projectDTO.image = null;

                if (projectDTO.description != null && (projectDTO.description.Equals("string") || projectDTO.description.Length == 0))
                    projectDTO.description = null;

                if (!await _validationService.CheckText(projectDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");
                ///
                if (projectDTO.investmentTargetCapital <= 0)
                    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                if (projectDTO.investedCapital <= 0)
                    throw new InvalidFieldException("investedCapital must be greater than 0!!!");

                if (projectDTO.sharedRevenue <= 0)
                    throw new InvalidFieldException("sharedRevenue must be greater than 0!!!");

                if (projectDTO.multiplier <= 0)
                    throw new InvalidFieldException("multiplier must be greater than 0!!!");

                if (projectDTO.duration <= 0)
                    throw new InvalidFieldException("duration must be greater than 0!!!");

                if (projectDTO.numOfStage <= 0)
                    throw new InvalidFieldException("numOfStage must be greater than 0!!!");

                if (projectDTO.remainAmount <= 0)
                    throw new InvalidFieldException("remainAmount must be greater than 0!!!");
                ///

                if (!await _validationService.CheckDate((projectDTO.startDate).ToString()))
                    throw new InvalidFieldException("Invalid startDate!!!");

                if (!await _validationService.CheckDate((projectDTO.endDate).ToString()))
                    throw new InvalidFieldException("Invalid endDate!!!");

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                if (projectDTO.status < 0 || projectDTO.status > 5)
                    throw new InvalidFieldException("Status must be 0(NOT_APPROVED_YET) or 1(DENIED) or 2(CALLING_FOR_INVESTMENT) or 3(CALLING_TIME_IS_OVER) or 4(ACTIVE) or 5(CLOSED)!!!");

                if (projectDTO.createBy != null && projectDTO.createBy.Length >= 0)
                {
                    if (projectDTO.createBy.Equals("string"))
                        projectDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (projectDTO.updateBy != null && projectDTO.updateBy.Length >= 0)
                {
                    if (projectDTO.updateBy.Equals("string"))
                        projectDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                projectDTO.isDeleted = false;

                Project dto = _mapper.Map<Project>(projectDTO);
                newId.id = await _projectRepository.CreateProject(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Project Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectById(Guid projectId)
        {
            int result;
            try
            {

                result = await _projectRepository.DeleteProjectById(projectId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<ProjectDTO>> GetAllProjects(int pageIndex, int pageSize)
        {
            try
            {
                List<Project> projectList = await _projectRepository.GetAllProjects(pageIndex, pageSize);
                List<ProjectDTO> list = _mapper.Map<List<ProjectDTO>>(projectList);
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<ProjectDTO> GetProjectById(Guid projectId)
        {
            ProjectDTO result;
            try
            {

                Project dto = await _projectRepository.GetProjectById(projectId);
                result = _mapper.Map<ProjectDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Project Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateProject(ProjectDTO projectDTO, Guid projectId)
        {
            int result;
            try
            {
                if (projectDTO.managerId == null || !await _validationService.CheckUUIDFormat(projectDTO.managerId))
                    throw new InvalidFieldException("Invalid managerId!!!");

                if (!await _validationService.CheckExistenceUserWithRole(ROLE_PROJECT_OWNER_ID, Guid.Parse(projectDTO.managerId)))
                    throw new NotFoundException("This ManagerId is not existed!!!");

                if (projectDTO.businessId == null || !await _validationService.CheckUUIDFormat(projectDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(projectDTO.businessId)))
                    throw new NotFoundException("This BusinessId is not existed!!!");

                //if (projectDTO.areaId == null || !await _validationService.CheckUUIDFormat(projectDTO.areaId))
                //    throw new InvalidFieldException("Invalid areaId!!!");

                //if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                //    throw new NotFoundException("This areaId is not existed!!!");

                if (!await _validationService.CheckText(projectDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (projectDTO.image != null && (projectDTO.image.Equals("string") || projectDTO.image.Length == 0))
                    projectDTO.image = null;

                if (projectDTO.description != null && (projectDTO.description.Equals("string") || projectDTO.description.Length == 0))
                    projectDTO.description = null;

                if (!await _validationService.CheckText(projectDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");
                ///
                if (projectDTO.investmentTargetCapital <= 0)
                    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                if (projectDTO.investedCapital <= 0)
                    throw new InvalidFieldException("investedCapital must be greater than 0!!!");

                if (projectDTO.sharedRevenue <= 0)
                    throw new InvalidFieldException("sharedRevenue must be greater than 0!!!");

                if (projectDTO.multiplier <= 0)
                    throw new InvalidFieldException("multiplier must be greater than 0!!!");

                if (projectDTO.duration <= 0)
                    throw new InvalidFieldException("duration must be greater than 0!!!");

                if (projectDTO.numOfStage <= 0)
                    throw new InvalidFieldException("numOfStage must be greater than 0!!!");

                if (projectDTO.remainAmount <= 0)
                    throw new InvalidFieldException("remainAmount must be greater than 0!!!");
                ///

                if (!await _validationService.CheckDate((projectDTO.startDate).ToString()))
                    throw new InvalidFieldException("Invalid startDate!!!");

                if (!await _validationService.CheckDate((projectDTO.endDate).ToString()))
                    throw new InvalidFieldException("Invalid endDate!!!");

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                if (projectDTO.status < 0 || projectDTO.status > 5)
                    throw new InvalidFieldException("Status must be 0(NOT_APPROVED_YET) or 1(DENIED) or 2(CALLING_FOR_INVESTMENT) or 3(CALLING_TIME_IS_OVER) or 4(ACTIVE) or 5(CLOSED)!!!");

                if (projectDTO.approvedBy != null && projectDTO.approvedBy.Length >= 0)
                {
                    if (projectDTO.approvedBy.Equals("string"))
                        projectDTO.approvedBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectDTO.approvedBy))
                        throw new InvalidFieldException("Invalid approvedBy!!!");
                }

                if (projectDTO.createBy != null && projectDTO.createBy.Length >= 0)
                {
                    if (projectDTO.createBy.Equals("string"))
                        projectDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (projectDTO.updateBy != null && projectDTO.updateBy.Length >= 0)
                {
                    if (projectDTO.updateBy.Equals("string"))
                        projectDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Project dto = _mapper.Map<Project>(projectDTO);
                result = await _projectRepository.UpdateProject(dto, projectId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
