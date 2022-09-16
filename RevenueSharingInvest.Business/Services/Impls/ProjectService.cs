using AutoMapper;
using Microsoft.VisualBasic;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
        private readonly IStageRepository _stageRepository;
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;

        private readonly IValidationService _validationService;
        private readonly IProjectTagService _projectTagService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectRepository projectRepository, 
            IFieldRepository fieldRepository, 
            IBusinessFieldRepository businessFieldRepository, 
            IUserRepository userRepository, 
            IBusinessRepository businessRepository,
            IAreaRepository areaRepository,
            IProjectEntityRepository projectEntityRepository,
            IStageRepository stageRepository,
            IPeriodRevenueRepository periodRevenueRepository,
            IProjectWalletRepository projectWalletRepository,
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
            _stageRepository = stageRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _projectWalletRepository = projectWalletRepository;

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
                if (thisUserObj.roleId.Equals("") || thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, null);
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);                 
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
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

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, fieldId, name, status, thisUserObj.roleId);
                }

                //else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                //{
                //    int[] statusNum = { 3, 5, 6, 7 };

                //    if (businessId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(businessId))
                //            throw new InvalidFieldException("Invalid businessId!!!");

                //        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                //            throw new NotFoundException("This businessId is not existed!!!");
                //    }

                //    if (areaId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(areaId))
                //            throw new InvalidFieldException("Invalid areaId!!!");

                //        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                //            throw new NotFoundException("This areaId is not existed!!!");
                //    }

                //    if (fieldId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(fieldId))
                //            throw new InvalidFieldException("Invalid fieldId!!!");

                //        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                //            throw new NotFoundException("This fieldId is not existed!!!");
                //    }

                //    if (status != null)
                //    {
                //        foreach (int item in statusNum)
                //        {
                //            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                //                statusCheck = true;
                //            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                //        }
                //        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                //        if (!statusCheck)
                //            throw new InvalidFieldException("INVESTOR can view Projects with status" + statusErrorMessage + " !!!");
                //    }

                //    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                //}

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateProject(CreateProjectDTO projectDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (projectDTO.managerId == null || !await _validationService.CheckUUIDFormat(projectDTO.managerId))
                    throw new InvalidFieldException("Invalid managerId!!!");

                if (!await _validationService.CheckExistenceUserWithRole(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"), Guid.Parse(projectDTO.managerId)))
                    throw new NotFoundException("This ManagerId is not existed!!!");

                if (!await _validationService.CheckManagerOfBusiness(Guid.Parse(projectDTO.managerId), Guid.Parse(currentUser.businessId)))
                    throw new InvalidFieldException("This manager does not belong to this business!!!");

                if (!(await _userRepository.GetUserById(Guid.Parse(projectDTO.managerId))).Status.Equals(ObjectStatusEnum.ACTIVE.ToString()))
                    throw new InvalidFieldException("This PROJECT_MANAGER's status must be ACTIVE!!!");

                if ((await _projectRepository.GetAllProjects(0, 0, null, projectDTO.managerId, null, null, null, null, RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))).Count() != 0)
                    throw new InvalidFieldException("This PROJECT_MANAGER has a project already!!!");

                if (projectDTO.fieldId == null || !await _validationService.CheckUUIDFormat(projectDTO.fieldId))
                    throw new InvalidFieldException("Invalid fieldId!!!");

                if (!await _validationService.CheckExistenceId("Field", Guid.Parse(projectDTO.fieldId)))
                    throw new NotFoundException("This fieldId is not existed!!!");

                if (!await _validationService.CheckProjectFieldInBusinessField(Guid.Parse(currentUser.businessId), Guid.Parse(projectDTO.fieldId)))
                    throw new InvalidFieldException("This fieldId is not suitable with this business!!!");

                if (projectDTO.areaId == null || !await _validationService.CheckUUIDFormat(projectDTO.areaId))
                    throw new InvalidFieldException("Invalid areaId!!!");

                if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                    throw new NotFoundException("This areaId is not existed!!!");

                if (!await _validationService.CheckText(projectDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (projectDTO.description != null && (projectDTO.description.Equals("string") || projectDTO.description.Length == 0))
                    projectDTO.description = null;

                if (!await _validationService.CheckText(projectDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");
                /////
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

                if (projectDTO.startDate != null && projectDTO.endDate != null)
                {
                    if ((DateAndTime.DateDiff(DateInterval.Day, DateTime.ParseExact(projectDTO.startDate, "dd/MM/yyyy HH:mm:ss", null), DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null))) < 0)
                        throw new InvalidFieldException("startDate can not bigger than endDate!!!");
                }

                if (!await _validationService.CheckDate((projectDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";

                if (!await _validationService.CheckDate((projectDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";               

                long totalDay = DateAndTime.DateDiff(DateInterval.Day, (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddDays(1), (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddMonths(projectDTO.duration)) + 1;
                int daysPerStage = ((int)totalDay) / projectDTO.numOfStage;
                int modDays = ((int)totalDay) - (daysPerStage * projectDTO.numOfStage);

                projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);
                projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                Project entity = _mapper.Map<Project>(projectDTO);

                entity.Status = Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0);
                entity.BusinessId = Guid.Parse(currentUser.businessId);
                entity.CreateBy = Guid.Parse(currentUser.userId);

                newId.id = await _projectRepository.CreateProject(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Project Object!");
                else
                {
                    //Tạo Stage và PeriodRevenue
                    Stage stage = new Stage();
                    PeriodRevenue periodRevenue = new PeriodRevenue();
                    string newStageId;
                    string newPeriodRevenueId;

                    stage.ProjectId = Guid.Parse(newId.id);
                    stage.Status = StageStatusEnum.UNDUE.ToString();
                    stage.CreateBy = Guid.Parse(currentUser.userId);
                    //stage.StartDate = entity.EndDate.AddDays(1);
                    stage.StartDate = DateTime.ParseExact(DateTime.Parse(entity.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(entity.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "00:00:00", "dd/MM/yyyy HH:mm:ss", null).AddDays(1);
                    stage.EndDate = DateTime.ParseExact(DateTime.Parse(stage.StartDate.AddDays(daysPerStage - 1).ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(stage.StartDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "23:59:59", "dd/MM/yyyy HH:mm:ss", null);

                    periodRevenue.ProjectId = Guid.Parse(newId.id);
                    periodRevenue.Status = StageStatusEnum.UNDUE.ToString();
                    periodRevenue.CreateBy = Guid.Parse(currentUser.userId);

                    for (int i = 1; i <= projectDTO.numOfStage - 1; i++)
                    {
                        stage.Name = projectDTO.name + " giai đoạn " + i;
                        newStageId = await _stageRepository.CreateStage(stage);
                        periodRevenue.StageId = Guid.Parse(newStageId);
                        newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                        stage.StartDate = stage.StartDate.AddDays(daysPerStage);
                        stage.EndDate = stage.EndDate.AddDays(daysPerStage);
                        
                    }
                    stage.Name = projectDTO.name + " giai đoạn " + projectDTO.numOfStage;
                    stage.EndDate = stage.EndDate.AddDays(modDays);
                    newStageId = await _stageRepository.CreateStage(stage);
                    periodRevenue.StageId = Guid.Parse(newStageId);
                    newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);

                    //Tạo ví B2, B3
                    await _projectWalletRepository.CreateProjectWallet(Guid.Parse(currentUser.userId), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B2")), Guid.Parse(currentUser.userId));
                    await _projectWalletRepository.CreateProjectWallet(Guid.Parse(currentUser.userId), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")), Guid.Parse(currentUser.userId));

                    //Update NumOfProject
                    await _businessRepository.UpdateBusinessNumOfProject(Guid.Parse(currentUser.businessId));
                }    
                

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
                if (thisUserObj.roleId.Equals("") || thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
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

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, fieldId, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, null, thisUserObj.userId, areaId, fieldId, name, status, thisUserObj.roleId);
                }

                //else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                //{
                //    int[] statusNum = { 3, 5, 6 ,7 };

                //    if (businessId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(businessId))
                //            throw new InvalidFieldException("Invalid businessId!!!");

                //        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                //            throw new NotFoundException("This businessId is not existed!!!");
                //    }

                //    if (areaId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(areaId))
                //            throw new InvalidFieldException("Invalid areaId!!!");

                //        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                //            throw new NotFoundException("This areaId is not existed!!!");
                //    }

                //    if (fieldId != null)
                //    {
                //        if (!await _validationService.CheckUUIDFormat(fieldId))
                //            throw new InvalidFieldException("Invalid fieldId!!!");

                //        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                //            throw new NotFoundException("This fieldId is not existed!!!");
                //    }

                //    if (status != null)
                //    {
                //        foreach (int item in statusNum)
                //        {
                //            if (status.Equals(Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item)))
                //                statusCheck = true;
                //            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(item) + "' or";
                //        }
                //        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                //        if (!statusCheck)
                //            throw new InvalidFieldException("INVESTOR can view Projects with status" + statusErrorMessage + " !!!");
                //    }

                //    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                //    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, fieldId, name, status, thisUserObj.roleId);
                //}                            

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
        public async Task<int> UpdateProject(UpdateProjectDTO projectDTO, Guid projectId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                GetProjectDTO getProject = _mapper.Map<GetProjectDTO>(await _projectRepository.GetProjectById(projectId));
                Project project = new Project();

                if (projectDTO.managerId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(projectDTO.managerId))
                        throw new InvalidFieldException("Invalid managerId!!!");

                    if (!await _validationService.CheckExistenceUserWithRole(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"), Guid.Parse(projectDTO.managerId)))
                        throw new NotFoundException("This ManagerId is not existed!!!");

                    if (!await _validationService.CheckManagerOfBusiness(Guid.Parse(projectDTO.managerId), Guid.Parse(currentUser.businessId)))
                        throw new InvalidFieldException("This manager does not belong to this business!!!");

                    if (!(await _userRepository.GetUserById(Guid.Parse(projectDTO.managerId))).Status.Equals(ObjectStatusEnum.ACTIVE.ToString()))
                        throw new InvalidFieldException("This PROJECT_MANAGER's status must be ACTIVE!!!");
                }

                if (projectDTO.fieldId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(projectDTO.fieldId))
                        throw new InvalidFieldException("Invalid fieldId!!!");

                    if (!await _validationService.CheckExistenceId("Field", Guid.Parse(projectDTO.fieldId)))
                        throw new NotFoundException("This fieldId is not existed!!!");

                    if (!await _validationService.CheckProjectFieldInBusinessField(Guid.Parse(currentUser.businessId), Guid.Parse(projectDTO.fieldId)))
                        throw new InvalidFieldException("This fieldId is not suitable with this business!!!");
                }
                
                if (projectDTO.areaId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(projectDTO.areaId))
                        throw new InvalidFieldException("Invalid areaId!!!");

                    if (!await _validationService.CheckExistenceId("Area", Guid.Parse(projectDTO.areaId)))
                        throw new NotFoundException("This areaId is not existed!!!");
                }

                if (projectDTO.name != null)
                {
                    if (!await _validationService.CheckText(projectDTO.name))
                        throw new InvalidFieldException("Invalid name!!!");
                }

                if (projectDTO.image != null)
                {
                    if (projectDTO.image.Equals("string") || projectDTO.image.Length == 0)
                        projectDTO.image = null;
                }

                if (projectDTO.description != null)
                {
                    if (projectDTO.description.Equals("string") || projectDTO.description.Length == 0)
                        projectDTO.description = null;
                }
                
                ///
                if (projectDTO.investmentTargetCapital != 0)
                {
                    if (projectDTO.investmentTargetCapital <= 0)
                        throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
                }

                if (projectDTO.sharedRevenue != 0)
                {
                    if (projectDTO.sharedRevenue <= 0)
                        throw new InvalidFieldException("sharedRevenue must be greater than 0!!!");
                }
                
                if (projectDTO.multiplier != 0)
                {
                    if (projectDTO.multiplier <= 0)
                        throw new InvalidFieldException("multiplier must be greater than 0!!!");
                }

                //Update [Stage] and [PeriodRevenue]
                if (projectDTO.duration != 0)
                {
                    if (projectDTO.duration <= 0)
                        throw new InvalidFieldException("duration must be greater than 0!!!");
                }
                else
                    projectDTO.duration = getProject.duration;


                if (projectDTO.numOfStage != 0)
                {
                    if (projectDTO.numOfStage <= 0)
                        throw new InvalidFieldException("numOfStage must be greater than 0!!!");
                }
                else
                    projectDTO.numOfStage = getProject.numOfStage;

                if (projectDTO.duration != 0 || projectDTO.numOfStage != 0 || projectDTO.startDate != null || projectDTO.endDate != null)
                {
                    if (projectDTO.startDate != null && projectDTO.endDate != null)
                    {
                        if ((DateAndTime.DateDiff(DateInterval.Day, DateTime.ParseExact(projectDTO.startDate, "dd/MM/yyyy HH:mm:ss", null), DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null))) < 0)
                            throw new InvalidFieldException("startDate can not bigger than endDate!!!");
                    }
                    
                    if ((projectDTO.startDate != null || projectDTO.endDate != null) && (projectDTO.startDate == null || projectDTO.endDate == null))
                        throw new InvalidFieldException("startDate and endDate must be update at the same time!!!");

                    if (projectDTO.startDate != null)
                    {
                        if (!await _validationService.CheckDate((projectDTO.startDate)))
                            throw new InvalidFieldException("Invalid startDate!!!");

                        projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";
                    }
                    else
                    {
                        projectDTO.startDate = await _validationService.FormatDateOutput(getProject.startDate);
                        projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";
                    }

                    if (projectDTO.endDate != null)
                    {
                        if (!await _validationService.CheckDate((projectDTO.endDate)))
                            throw new InvalidFieldException("Invalid endDate!!!");

                        projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";
                    }
                    else
                    {
                        projectDTO.endDate = await _validationService.FormatDateOutput(getProject.endDate);
                        projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";
                    }

                    //Xóa [Stage] và [PeriodRevenue] cũ
                    await _periodRevenueRepository.DeletePeriodRevenueByProjectId(projectId);
                    await _stageRepository.DeleteStageByProjectId(projectId);

                    //Tạo lại [Stage] và [PeriodRevenue]

                    long totalDay = DateAndTime.DateDiff(DateInterval.Day, (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddDays(1), (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddMonths(projectDTO.duration)) + 1;
                    int daysPerStage = ((int)totalDay) / projectDTO.numOfStage;
                    int modDays = ((int)totalDay) - (daysPerStage * projectDTO.numOfStage);

                    projectDTO.name = getProject.name;
                    projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);
                    projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                    project = _mapper.Map<Project>(projectDTO);
                   
                    Stage stage = new Stage();
                    PeriodRevenue periodRevenue = new PeriodRevenue();
                    string newStageId;
                    string newPeriodRevenueId;

                    stage.ProjectId = projectId;
                    stage.Status = StageStatusEnum.UNDUE.ToString();
                    stage.CreateBy = Guid.Parse(currentUser.userId);
                    stage.StartDate = DateTime.ParseExact(DateTime.Parse(project.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(project.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "00:00:00", "dd/MM/yyyy HH:mm:ss", null).AddDays(1);
                    stage.EndDate = DateTime.ParseExact(DateTime.Parse(stage.StartDate.AddDays(daysPerStage - 1).ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(stage.StartDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "23:59:59", "dd/MM/yyyy HH:mm:ss", null);

                    periodRevenue.ProjectId = projectId;
                    periodRevenue.Status = StageStatusEnum.UNDUE.ToString();
                    periodRevenue.CreateBy = Guid.Parse(currentUser.userId);

                    for (int i = 1; i <= projectDTO.numOfStage - 1; i++)
                    {
                        stage.Name = projectDTO.name + " giai đoạn " + i;
                        newStageId = await _stageRepository.CreateStage(stage);
                        periodRevenue.StageId = Guid.Parse(newStageId);
                        newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                        stage.StartDate = stage.StartDate.AddDays(daysPerStage);
                        stage.EndDate = stage.EndDate.AddDays(daysPerStage);

                    }
                    stage.Name = projectDTO.name + " giai đoạn " + projectDTO.numOfStage;
                    stage.EndDate = stage.EndDate.AddDays(modDays);
                    newStageId = await _stageRepository.CreateStage(stage);
                    periodRevenue.StageId = Guid.Parse(newStageId);
                    newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                    
                }
                else
                {
                    projectDTO.startDate = null;
                    projectDTO.endDate = null;
                    project = _mapper.Map<Project>(projectDTO);
                }                

                if (projectDTO.investmentTargetCapital == 0)
                    project.InvestmentTargetCapital = getProject.investmentTargetCapital;
                else
                    project.RemainAmount = project.InvestmentTargetCapital;

                if (projectDTO.sharedRevenue == 0)
                    project.SharedRevenue = getProject.sharedRevenue;

                if (projectDTO.multiplier == 0)
                    project.Multiplier = getProject.multiplier;

                //if (projectDTO.duration == 0)
                //    project.Duration = getProject.duration;

                //if (projectDTO.numOfStage == 0)
                //    project.NumOfStage = getProject.numOfStage;

                if (projectDTO.image != null)
                {
                    project.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, RoleDictionary.role.GetValueOrDefault("ADMIN"));
                }
                project.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _projectRepository.UpdateProject(project, projectId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateProjectStatus(Guid projectId, string status, ThisUserObj currentUser)
        {
            int result;
            try
            {
                Project project = await _projectRepository.GetProjectById(projectId);
                if (project == null)
                    throw new InvalidFieldException("There are no Project has this Id!!!");

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (!project.Status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                        throw new InvalidFieldException("ADMIN can update Project's status from WAITING_FOR_APPROVAL to DENIED or CALLING_FOR_INVESTMENT!!!");

                    if (!status.Equals(ProjectStatusEnum.DENIED.ToString()) && !status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString()))
                        throw new InvalidFieldException("Status must be DENIED or CALLING_FOR_INVESTMENT!!!");
                }
                else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    if (!project.BusinessId.ToString().Equals(currentUser.businessId))
                        throw new InvalidFieldException("The Project with this businessId is not match with this PROJECT_MANAGER's businessId!!!");

                    if (!project.Status.Equals(ProjectStatusEnum.DRAFT.ToString()))
                        throw new InvalidFieldException("PROJECT_MANAGER can update Project's status from DRAFT to WAITING_FOR_APPROVAL!!!");

                    if (!status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                        throw new InvalidFieldException("Status must be WAITING_FOR_APPROVAL!!!");
                }

                result = await _projectRepository.UpdateProjectStatus(projectId, status, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update Project status!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET INVESTED PROJECTS
        public async Task<AllProjectDTO> GetInvestedProjects(int pageIndex, int pageSize, ThisUserObj thisUserObj)
        {
            AllProjectDTO result = new AllProjectDTO();
            result.listOfProject = new List<GetProjectDTO>();
            List<Project> listEntity = new List<Project>();

            try
            {
                result.numOfProject = await _projectRepository.CountInvestedProjects(Guid.Parse(thisUserObj.roleId));
                listEntity = await _projectRepository.GetInvestedProjects(pageIndex, pageSize, Guid.Parse(thisUserObj.roleId));

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
    }
}
