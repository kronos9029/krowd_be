using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
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
        private readonly IFieldRepository _fieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly String ROLE_PROJECT_MANAGER_ID = "2d80393a-3a3d-495d-8dd7-f9261f85cc8f";

        public ProjectService(
            IProjectRepository projectRepository, 
            IFieldRepository fieldRepository, 
            IBusinessFieldRepository businessFieldRepository, 
            IUserRepository userRepository, 
            IBusinessRepository businessRepository, 
            IValidationService validationService, 
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _fieldRepository = fieldRepository;
            _businessFieldRepository = businessFieldRepository;
            _userRepository = userRepository;
            _businessRepository = businessRepository;
            _validationService = validationService;
            _fileUploadService = fileUploadService;
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

                if (projectDTO.businessId == null || !await _validationService.CheckUUIDFormat(projectDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(projectDTO.businessId)))
                    throw new NotFoundException("This BusinessId is not existed!!!");

                if (projectDTO.managerId == null || !await _validationService.CheckUUIDFormat(projectDTO.managerId))
                    throw new InvalidFieldException("Invalid managerId!!!");

                if (!await _validationService.CheckExistenceUserWithRole(ROLE_PROJECT_MANAGER_ID, Guid.Parse(projectDTO.managerId)))
                    throw new NotFoundException("This ManagerId is not existed!!!");
                
                if (!await _validationService.CheckManagerOfBusiness(Guid.Parse(projectDTO.managerId), Guid.Parse(projectDTO.businessId)))
                    throw new InvalidFieldException("This manager does not belong to this business!!!");

                if (projectDTO.fieldId == null || !await _validationService.CheckUUIDFormat(projectDTO.fieldId))
                    throw new InvalidFieldException("Invalid fieldId!!!");

                if (!await _validationService.CheckExistenceId("Field", Guid.Parse(projectDTO.fieldId)))
                    throw new NotFoundException("This fieldId is not existed!!!");

                if (!await _validationService.CheckProjectFieldInBusinessField(Guid.Parse(projectDTO.businessId), Guid.Parse(projectDTO.fieldId)))
                    throw new InvalidFieldException("This fieldId is not suitable with this business!!!");

                if (projectDTO.areaId == null || !await _validationService.CheckUUIDFormat(projectDTO.areaId))
                    throw new InvalidFieldException("Invalid areaId!!!");

                if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                    throw new NotFoundException("This areaId is not existed!!!");

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

                if (!await _validationService.CheckDate((projectDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);

                if (!await _validationService.CheckDate((projectDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                if (projectDTO.status < 0 || projectDTO.status > 5)
                    throw new InvalidFieldException("Status must be 0(NOT_APPROVED_YET) or 1(DENIED) or 2(CALLING_FOR_INVESTMENT) or 3(CALLING_TIME_IS_OVER) or 4(ACTIVE) or 5(CLOSED)!!!");

                projectDTO.approvedBy = null;

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

                Project entity = _mapper.Map<Project>(projectDTO);

                if(projectDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, projectDTO.createBy);
                }

                newId.id = await _projectRepository.CreateProject(entity);
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
        public async Task<AllProjectDTO> GetAllProjects(int pageIndex, int pageSize, string businessId, string managerId, string temp_field_role)
        {
            try
            {
                AllProjectDTO result = new AllProjectDTO();
                result.listOfProject = new List<ProjectDetailDTO>();
                ProjectDetailDTO resultItem = new ProjectDetailDTO();

                if (businessId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(businessId))
                        throw new InvalidFieldException("Invalid businessId!!!");

                    if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                        throw new NotFoundException("This businessId is not existed!!!");
                }

                if (managerId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(managerId))
                        throw new InvalidFieldException("Invalid managerId!!!");

                    if (!await _validationService.CheckExistenceUserWithRole(ROLE_PROJECT_MANAGER_ID, Guid.Parse(managerId)))
                        throw new NotFoundException("This managerId is not existed!!!");
                }

                result.numOfProject = await _projectRepository.CountProject(businessId, managerId, temp_field_role);

                List<Project> listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, managerId, temp_field_role);
                List<ProjectDTO> listDTO = _mapper.Map<List<ProjectDTO>>(listEntity);

                foreach (ProjectDTO item in listDTO)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    if (item.approvedDate != null)
                    {
                        item.approvedDate = await _validationService.FormatDateOutput(item.approvedDate);
                    }
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    resultItem = _mapper.Map<ProjectDetailDTO>(item);
                    resultItem.manager = _mapper.Map<UserDTO>(await _userRepository.GetUserById(Guid.Parse(item.managerId)));
                    resultItem.business = _mapper.Map<BusinessDTO>(await _businessRepository.GetBusinessById(Guid.Parse(item.businessId)));
                    resultItem.field = _mapper.Map<FieldDTO>(await _fieldRepository.GetFieldById(Guid.Parse(item.fieldId)));
                    result.listOfProject.Add(resultItem);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<ProjectDetailDTO> GetProjectById(Guid projectId)
        {
            ProjectDetailDTO result;
            try
            {

                Project project = await _projectRepository.GetProjectById(projectId);
                ProjectDTO projectDTO = _mapper.Map<ProjectDTO>(project);
                if (projectDTO == null)
                    throw new NotFoundException("No Project Object Found!");

                projectDTO.startDate = await _validationService.FormatDateOutput(projectDTO.startDate);
                projectDTO.endDate = await _validationService.FormatDateOutput(projectDTO.endDate);
                if (projectDTO.approvedDate != null)
                {
                    projectDTO.approvedDate = await _validationService.FormatDateOutput(projectDTO.approvedDate);
                }
                projectDTO.createDate = await _validationService.FormatDateOutput(projectDTO.createDate);
                projectDTO.updateDate = await _validationService.FormatDateOutput(projectDTO.updateDate);

                result = _mapper.Map<ProjectDetailDTO>(projectDTO);
                result.manager = _mapper.Map<UserDTO>(await _userRepository.GetUserById(Guid.Parse(projectDTO.managerId)));
                result.business = _mapper.Map<BusinessDTO>(await _businessRepository.GetBusinessById(Guid.Parse(projectDTO.businessId)));
                result.field = _mapper.Map<FieldDTO>(await _fieldRepository.GetFieldById(Guid.Parse(projectDTO.fieldId)));

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
                if (projectDTO.businessId == null || !await _validationService.CheckUUIDFormat(projectDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(projectDTO.businessId)))
                    throw new NotFoundException("This BusinessId is not existed!!!");

                if (projectDTO.managerId == null || !await _validationService.CheckUUIDFormat(projectDTO.managerId))
                    throw new InvalidFieldException("Invalid managerId!!!");

                if (!await _validationService.CheckExistenceUserWithRole(ROLE_PROJECT_MANAGER_ID, Guid.Parse(projectDTO.managerId)))
                    throw new NotFoundException("This ManagerId is not existed!!!");

                if (!await _validationService.CheckManagerOfBusiness(Guid.Parse(projectDTO.managerId), Guid.Parse(projectDTO.businessId)))
                    throw new InvalidFieldException("This manager does not belong to this business!!!");

                if (projectDTO.fieldId == null || !await _validationService.CheckUUIDFormat(projectDTO.fieldId))
                    throw new InvalidFieldException("Invalid fieldId!!!");

                if (!await _validationService.CheckExistenceId("Field", Guid.Parse(projectDTO.fieldId)))
                    throw new NotFoundException("This fieldId is not existed!!!");

                if (!await _validationService.CheckProjectFieldInBusinessField(Guid.Parse(projectDTO.businessId), Guid.Parse(projectDTO.fieldId)))
                    throw new InvalidFieldException("This fieldId is not suitable with this business!!!");
       
                if (projectDTO.areaId == null || !await _validationService.CheckUUIDFormat(projectDTO.areaId))
                    throw new InvalidFieldException("Invalid areaId!!!");

                if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                    throw new NotFoundException("This areaId is not existed!!!");

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

                if (!await _validationService.CheckDate((projectDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);

                if (!await _validationService.CheckDate((projectDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                if (projectDTO.status < 0 || projectDTO.status > 5)
                    throw new InvalidFieldException("Status must be 0(NOT_APPROVED_YET) or 1(DENIED) or 2(CALLING_FOR_INVESTMENT) or 3(CALLING_TIME_IS_OVER) or 4(ACTIVE) or 5(CLOSED)!!!");

                projectDTO.approvedBy = null;

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

                Project entity = _mapper.Map<Project>(projectDTO);

                if (projectDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, projectDTO.updateBy);
                }

                result = await _projectRepository.UpdateProject(entity, projectId);
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
