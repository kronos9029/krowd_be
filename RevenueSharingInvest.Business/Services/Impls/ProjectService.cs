using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
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
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IProjectEntityRepository _projectEntityRepository;

        private readonly IValidationService _validationService;
        private readonly IProjectTagService _projectTagService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";
        private readonly String ROLE_PROJECT_MANAGER_ID = "2d80393a-3a3d-495d-8dd7-f9261f85cc8f";
        private readonly String ROLE_INVESTOR_ID = "ad5f37da-ca48-4dc5-9f4b-963d94b535e6";

        public ProjectService(
            IProjectRepository projectRepository, 
            IFieldRepository fieldRepository, 
            IBusinessFieldRepository businessFieldRepository, 
            IUserRepository userRepository, 
            IBusinessRepository businessRepository,
            IAreaRepository areaRepository,
            IProjectEntityRepository projectEntityRepository,
            IValidationService validationService,
            IProjectTagService projectTagService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _fieldRepository = fieldRepository;
            _businessFieldRepository = businessFieldRepository;
            _userRepository = userRepository;
            _businessRepository = businessRepository;
            _areaRepository = areaRepository;
            _projectEntityRepository = projectEntityRepository;

            _validationService = validationService;
            _projectTagService = projectTagService;
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

        //COUNT PROJECTS
        public async Task<ProjectCountDTO> CountProjects(string businessId, string areaId, string fieldId, string name, string status, ThisUserObj thisUserObj)
        {
            ProjectCountDTO result = new ProjectCountDTO();

            bool statusCheck = false;
            string statusErrorMessage = "";
            try
            {
                if (thisUserObj.roleId.Equals(""))
                {
                    int[] statusNum = { 3 };
                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("GUEST can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, null);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 1, 2, 3, 4, 5, 6, 7 };

                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("ADMIN can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);                 
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7 };
                    if (businessId != null && !businessId.Equals(thisUserObj.businessId))
                        throw new InvalidFieldException("businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                    businessId = thisUserObj.businessId;

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("BUSINESS_MANAGER can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7 };

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("PROJECT_MANAGER can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    int[] statusNum = { 3, 5, 6, 7 };

                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("INVESTOR can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, thisUserObj.investorId, name, status, thisUserObj.roleId);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateProject(CreateUpdateProjectDTO projectDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (projectDTO.businessId == null || !await _validationService.CheckUUIDFormat(projectDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(projectDTO.businessId)))
                    throw new NotFoundException("This BusinessId is not existed!!!");

                if (!projectDTO.businessId.Equals(currentUser.businessId))
                    throw new InvalidFieldException("This businessId is not match with creator businessId!!!");

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

                if (projectDTO.sharedRevenue <= 0)
                    throw new InvalidFieldException("sharedRevenue must be greater than 0!!!");

                if (projectDTO.multiplier <= 0)
                    throw new InvalidFieldException("multiplier must be greater than 0!!!");

                if (projectDTO.duration <= 0)
                    throw new InvalidFieldException("duration must be greater than 0!!!");

                if (projectDTO.numOfStage <= 0)
                    throw new InvalidFieldException("numOfStage must be greater than 0!!!");

                if (!await _validationService.CheckDate((projectDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);

                if (!await _validationService.CheckDate((projectDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                Project entity = _mapper.Map<Project>(projectDTO);

                entity.Status = Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0);

                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                if (projectDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, ROLE_ADMIN_ID);
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
        public async Task<AllProjectDTO> GetAllProjects
        (
            int pageIndex, 
            int pageSize, 
            string businessId, 
            string areaId, 
            string fieldId, 
            string name, 
            string status, 
            ThisUserObj thisUserObj
        )
        {
            AllProjectDTO result = new AllProjectDTO();
            result.listOfProject = new List<GetProjectDTO>();
            List<Project> listEntity = new List<Project>();

            bool statusCheck = false;
            string statusErrorMessage = "";

            try
            {
                if (thisUserObj.roleId.Equals(""))
                {
                    int[] statusNum = { 3 };
                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("GUEST can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 1, 2, 3, 4, 5, 6, 7 };

                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("ADMIN can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7 };
                    if (businessId != null && !businessId.Equals(thisUserObj.businessId))
                        throw new InvalidFieldException("businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                    businessId = thisUserObj.businessId;

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("BUSINESS_MANAGER can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7 };                   

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("PROJECT_MANAGER can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, fieldId, null, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, null, thisUserObj.userId, areaId, fieldId, null, name, status, thisUserObj.roleId);
                }

                else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    int[] statusNum = { 3, 5, 6 ,7 };

                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (fieldId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(fieldId))
                            throw new InvalidFieldException("Invalid fieldId!!!");

                        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                            throw new NotFoundException("This fieldId is not existed!!!");
                    }

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("INVESTOR can view Projects with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, thisUserObj.investorId, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, thisUserObj.investorId, name, status, thisUserObj.roleId);
                }                            

                List<GetProjectDTO> listDTO = _mapper.Map<List<GetProjectDTO>>(listEntity);

                foreach (GetProjectDTO item in listDTO)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    if (item.approvedDate != null)
                    {
                        item.approvedDate = await _validationService.FormatDateOutput(item.approvedDate);
                    }
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    item.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByProjectId(Guid.Parse(item.id)));
                    if (item.business != null)
                    {
                        item.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(item.business.id)));
                        item.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(item.business.id)));
                    }
                    item.manager = _mapper.Map<ProjectManagerUserDTO>(await _userRepository.GetProjectManagerByProjectId(Guid.Parse(item.id)));
                    item.field = _mapper.Map<FieldDTO>(await _fieldRepository.GetProjectFieldByProjectId(Guid.Parse(item.id)));
                    item.area = _mapper.Map<AreaDTO>(await _areaRepository.GetAreaByProjectId(Guid.Parse(item.id)));
                    item.projectEntity = new List<TypeProjectEntityDTO>();              
                    for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                    {
                        TypeProjectEntityDTO typeProjectEntityDTO = new TypeProjectEntityDTO();
                        typeProjectEntityDTO.type = Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type);
                        typeProjectEntityDTO.typeItemList = _mapper.Map<List<ProjectComponentProjectEntityDTO>>(await _projectEntityRepository.GetProjectEntityByProjectIdAndType(Guid.Parse(item.id), Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)));
                        item.projectEntity.Add(typeProjectEntityDTO);
                    }                    
                    item.memberList = _mapper.Map<List<ProjectMemberUserDTO>>(await _userRepository.GetProjectMembers(Guid.Parse(item.id)));
                    item.tagList = await _projectTagService.GetProjectTagList(item);

                    result.listOfProject.Add(item);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetProjectDTO> GetProjectById(Guid projectId)
        {
            try
            {
                Project project = await _projectRepository.GetProjectById(projectId);
                GetProjectDTO projectDTO = _mapper.Map<GetProjectDTO>(project);
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

                projectDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByProjectId(Guid.Parse(projectDTO.id)));
                if (projectDTO.business != null)
                {
                    projectDTO.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(projectDTO.business.id)));
                    projectDTO.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(projectDTO.business.id)));
                }
                projectDTO.manager = _mapper.Map<ProjectManagerUserDTO>(await _userRepository.GetProjectManagerByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.field = _mapper.Map<FieldDTO>(await _fieldRepository.GetProjectFieldByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.area = _mapper.Map<AreaDTO>(await _areaRepository.GetAreaByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.projectEntity = new List<TypeProjectEntityDTO>();
                for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                {
                    TypeProjectEntityDTO typeProjectEntityDTO = new TypeProjectEntityDTO();
                    typeProjectEntityDTO.type = Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type);
                    typeProjectEntityDTO.typeItemList = _mapper.Map<List<ProjectComponentProjectEntityDTO>>(await _projectEntityRepository.GetProjectEntityByProjectIdAndType(Guid.Parse(projectDTO.id), Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)));
                    projectDTO.projectEntity.Add(typeProjectEntityDTO);
                }
                projectDTO.memberList = _mapper.Map<List<ProjectMemberUserDTO>>(await _userRepository.GetProjectMembers(Guid.Parse(projectDTO.id)));
                projectDTO.tagList = await _projectTagService.GetProjectTagList(projectDTO);

                return projectDTO;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<BusinessProjectDTO>> GetBusinessProjectsToAuthor(Guid businessId)
        {
            try
            {
                return await _projectRepository.GetBusinessProjectsToAuthor(businessId);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateProject(CreateUpdateProjectDTO projectDTO, Guid projectId)
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

                if (projectDTO.sharedRevenue <= 0)
                    throw new InvalidFieldException("sharedRevenue must be greater than 0!!!");

                if (projectDTO.multiplier <= 0)
                    throw new InvalidFieldException("multiplier must be greater than 0!!!");

                if (projectDTO.duration <= 0)
                    throw new InvalidFieldException("duration must be greater than 0!!!");

                if (projectDTO.numOfStage <= 0)
                    throw new InvalidFieldException("numOfStage must be greater than 0!!!");

                //if (projectDTO.remainAmount <= 0)
                //    throw new InvalidFieldException("remainAmount must be greater than 0!!!");
                ///

                if (!await _validationService.CheckDate((projectDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);

                if (!await _validationService.CheckDate((projectDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                if (!await _validationService.CheckText(projectDTO.businessLicense))
                    throw new InvalidFieldException("Invalid businessLicense!!!");

                //if (projectDTO.status < 0 || projectDTO.status > 5)
                //    throw new InvalidFieldException("Status must be 0(NOT_APPROVED_YET) or 1(DENIED) or 2(CALLING_FOR_INVESTMENT) or 3(CALLING_TIME_IS_OVER) or 4(ACTIVE) or 5(CLOSED)!!!");

                //projectDTO.approvedBy = null;

                //if (projectDTO.createBy != null && projectDTO.createBy.Length >= 0)
                //{
                //    if (projectDTO.createBy.Equals("string"))
                //        projectDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(projectDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (projectDTO.updateBy != null && projectDTO.updateBy.Length >= 0)
                //{
                //    if (projectDTO.updateBy.Equals("string"))
                //        projectDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(projectDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                Project entity = _mapper.Map<Project>(projectDTO);

                if (projectDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, ROLE_ADMIN_ID);
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
