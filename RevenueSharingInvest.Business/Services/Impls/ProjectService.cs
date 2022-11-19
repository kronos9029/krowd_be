using AutoMapper;
using FirebaseAdmin.Messaging;
using Google.Api.Gax.ResourceNames;
using Hangfire;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Business.Services.Extensions.Security;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;

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
        private readonly IPeriodRevenueHistoryRepository _periodRevenueHistoryRepository;
        private readonly IPackageRepository _packageRepository;       
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IDailyReportRepository _dailyReportRepository;
        private readonly IBillRepository _billRepository;

        private readonly IValidationService _validationService;
        private readonly IProjectTagService _projectTagService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IStageService _stageService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

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
            IPeriodRevenueHistoryRepository periodRevenueHistoryRepository,
            IProjectWalletRepository projectWalletRepository,
            IPackageRepository packageRepository,
            IPaymentRepository paymentRepository,
            IInvestmentRepository investmentRepository,
            IInvestorWalletRepository investorWalletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IDailyReportRepository dailyReportRepository,
            IBillRepository billRepository,

            IValidationService validationService,
            IProjectTagService projectTagService,
            IFileUploadService fileUploadService,
            IStageService stageService,
            IBackgroundJobClient backgroundJobClient,
            IMapper mapper,
            IDistributedCache cache)
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
            _periodRevenueHistoryRepository = periodRevenueHistoryRepository;
            _packageRepository = packageRepository;
            _paymentRepository = paymentRepository;
            _projectWalletRepository = projectWalletRepository;
            _investmentRepository = investmentRepository;
            _investorWalletRepository = investorWalletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _dailyReportRepository = dailyReportRepository;
            _billRepository = billRepository;

            _validationService = validationService;
            _projectTagService = projectTagService;
            _fileUploadService = fileUploadService;
            _stageService = stageService;
            _backgroundJobClient = backgroundJobClient;
            _mapper = mapper;
            _cache = cache;
        }

        //COUNT PROJECTS
        public async Task<ProjectCountDTO> CountProjects(string businessId, string areaId, List<string> listFieldId, double investmentTargetCapital, string name, string status, ThisUserObj thisUserObj)
        {
            ProjectCountDTO result = new ProjectCountDTO();

            bool statusCheck = false;
            string statusErrorMessage = "";
            try
            {
                if (thisUserObj.roleId.Equals("") || thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    int[] statusNum = { 4 };
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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.investorRoleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 1, 2, 3, 4, 5, 6, 7, 8 };

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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);                 
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                }

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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

                //if ((await _projectRepository.GetAllProjects(0, 0, null, projectDTO.managerId, null, null, null, null, RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))).Count() != 0)
                //    throw new InvalidFieldException("This PROJECT_MANAGER has a project already!!!");

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

                if (projectDTO.sharedRevenue <= 0 || projectDTO.sharedRevenue > 100)
                    throw new InvalidFieldException("sharedRevenue must be greater than 0 and less than 100!!!");

                if (projectDTO.multiplier <= 0 || projectDTO.multiplier > 100)
                    throw new InvalidFieldException("multiplier must be greater than 0 and less than 100!!!");

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
                entity.AccessKey = GenerateSecurityKey.GenerateAccessKey();

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
                    stage.CreateBy = Guid.Parse(currentUser.userId);
                    //stage.StartDate = entity.EndDate.AddDays(1);
                    stage.StartDate = DateTime.ParseExact(DateTime.Parse(entity.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(entity.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "00:00:00", "dd/MM/yyyy HH:mm:ss", null).AddDays(11);
                    stage.EndDate = DateTime.ParseExact(DateTime.Parse(stage.StartDate.AddDays(daysPerStage - 1).ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(stage.StartDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "23:59:59", "dd/MM/yyyy HH:mm:ss", null);

                    periodRevenue.ProjectId = Guid.Parse(newId.id);
                    periodRevenue.CreateBy = Guid.Parse(currentUser.userId);

                    for (int i = 1; i <= projectDTO.numOfStage - 1; i++)
                    {
                        stage.Name = "Kỳ " + i;
                        newStageId = await _stageRepository.CreateStage(stage);
                        periodRevenue.StageId = Guid.Parse(newStageId);
                        newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                        stage.StartDate = stage.StartDate.AddDays(daysPerStage);
                        stage.EndDate = stage.EndDate.AddDays(daysPerStage);
                        
                    }
                    stage.Name = "Kỳ " + projectDTO.numOfStage;
                    stage.EndDate = stage.EndDate.AddDays(modDays);
                    newStageId = await _stageRepository.CreateStage(stage);
                    periodRevenue.StageId = Guid.Parse(newStageId);
                    newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);

                    //Tạo ví P3, P4 cho PROJECT_MANAGER
                    ProjectWallet projectWallet = new ProjectWallet();
                    projectWallet.ProjectManagerId = Guid.Parse(currentUser.userId);
                    projectWallet.ProjectId = Guid.Parse(newId.id);
                    projectWallet.CreateBy = Guid.Parse(currentUser.userId);

                    projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P3"));
                    await _projectWalletRepository.CreateProjectWallet(projectWallet);
                    projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4"));
                    await _projectWalletRepository.CreateProjectWallet(projectWallet);

                    //Tạo EXTENSION ProjectEntity
                    User user = await _userRepository.GetUserById(Guid.Parse(projectDTO.managerId));
                    Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(entity.BusinessId);

                    ProjectEntity extension = new()
                    {
                        ProjectId = Guid.Parse(newId.id),
                        Type = ProjectEntityEnum.EXTENSION.ToString(),
                        CreateBy = Guid.Parse(currentUser.userId),
                        Title = "Doanh nghiệp",
                        Content = business.Name,
                        Description = "Email: " + business.Email,
                        Priority = 1
                    };
                    await _projectEntityRepository.CreateProjectEntity(extension);
                    extension.Title = "Chủ dự án";
                    extension.Content = user.FirstName + " " + user.LastName;
                    extension.Description = "Liên hệ: " + user.PhoneNum;
                    extension.Priority = 2;
                    await _projectEntityRepository.CreateProjectEntity(extension);
                    extension.Title = "Ngày kết thúc gọi vốn";
                    extension.Content = (await _validationService.FormatDateOutput(projectDTO.endDate)).Remove(projectDTO.endDate.Length - 8);
                    extension.Description = "";
                    extension.Priority = 3;
                    await _projectEntityRepository.CreateProjectEntity(extension);
                    extension.Title = "Ngày dự đoán đóng dự án";
                    extension.Content = (await _validationService.FormatDateOutput(stage.EndDate.ToString())).Remove(projectDTO.endDate.Length - 8);
                    extension.Description = "";
                    extension.Priority = 4;
                    await _projectEntityRepository.CreateProjectEntity(extension);

                    //Tạo ABOUT ProjectEntity
                    ProjectEntity about = new()
                    {
                        ProjectId = Guid.Parse(newId.id),
                        Type = ProjectEntityEnum.ABOUT.ToString(),
                        CreateBy = Guid.Parse(currentUser.userId),
                        Title = "Facebook",
                        Priority = 1
                    };
                    await _projectEntityRepository.CreateProjectEntity(about);
                    about.Title = "YouTube";
                    about.Priority = 2;
                    await _projectEntityRepository.CreateProjectEntity(about);
                    about.Title = "Instagram";
                    about.Priority = 3;
                    await _projectEntityRepository.CreateProjectEntity(about);

                    //Update NumOfProject
                    await _businessRepository.UpdateBusinessNumOfProject(Guid.Parse(currentUser.businessId));


                    NotificationDetailDTO noti = new()
                    {
                        Title = currentUser.fullName + " đã tạo dự án mới: " + entity.Name,
                        EntityId = newId.id
                    };

                    List<Guid> admins = await _userRepository.GetUsersIdByRoleIdAndBusinessId(Guid.Parse(currentUser.adminRoleId),"");
                    List<Guid> businessManagers = await _userRepository.GetUsersIdByRoleIdAndBusinessId(Guid.Parse(currentUser.businessManagerRoleId), currentUser.businessId);
                    var combinedList = (List<Guid>)admins.Union(businessManagers).ToList();

                    for (int i = 0; i < combinedList.Count; i++)
                    {
                        await DistributedCacheExtensions.UpdateNotification(_cache, combinedList[i].ToString(), noti);
                    }
                }
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectById(Guid projectId)
        {
            int result;
            try
            {
                //Xóa PeriodRevenueHistory
                await _periodRevenueHistoryRepository.DeletePeriodRevenueHistoryByProjectId(projectId);
                //Xóa PeriodRevenue
                await _periodRevenueRepository.DeletePeriodRevenueByProjectId(projectId);
                //Xóa Bill
                await _billRepository.DeleteBillByProjectId(projectId);
                //Xóa DailyReport
                await _dailyReportRepository.DeleteDailyReportByProjectId(projectId);
                //Xóa Stage
                await _stageRepository.DeleteStageByProjectId(projectId);
                //Xóa ProjectEntity
                await _projectEntityRepository.DeleteProjectEntityByProjectId(projectId);
                //Xóa Package
                await _packageRepository.DeletePackageByProjectId(projectId);
                ////Xóa ProjectWallet
                //result = await _projectWalletRepository.DeleteProjectWalletByProjectId(projectId);
                //Xóa Project
                result = await _projectRepository.DeleteProjectById(projectId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Project Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
            List<string> listFieldId,
            double investmentTargetCapital,
            string name, 
            string status, 
            ThisUserObj thisUserObj
        )
        {
            AllProjectDTO result = new AllProjectDTO();
            result.listOfProject = new List<BasicProjectDTO>();
            List<Project> listEntity = new List<Project>();

            bool statusCheck = false;
            string statusErrorMessage = "";

            try
            {
                if (thisUserObj.roleId.Equals("") || thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    int[] statusNum = { 4 };
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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }                       
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.investorRoleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.investorRoleId);
                }

                else if (thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 1, 2, 3, 4, 5, 6, 7, 8 };

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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                }

                else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
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

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, businessId, null, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                }

                else if(thisUserObj.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };                   

                    if (areaId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(areaId))
                            throw new InvalidFieldException("Invalid areaId!!!");

                        if (!await _validationService.CheckExistenceId("Area", Guid.Parse(areaId)))
                            throw new NotFoundException("This areaId is not existed!!!");
                    }

                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            if (!await _validationService.CheckUUIDFormat(fieldId))
                                throw new InvalidFieldException("Invalid fieldId " + fieldId + "!!!");

                            if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                                throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                        }
                    }

                    if (investmentTargetCapital != 0)
                    {
                        if (investmentTargetCapital < 0)
                            throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");
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

                    result.numOfProject = await _projectRepository.CountProject(null, thisUserObj.userId, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                    listEntity = await _projectRepository.GetAllProjects(pageIndex, pageSize, null, thisUserObj.userId, areaId, listFieldId, investmentTargetCapital, name, status, thisUserObj.roleId);
                }               

                List<BasicProjectDTO> listDTO = _mapper.Map<List<BasicProjectDTO>>(listEntity);
                Data.Models.Entities.Business business = new Data.Models.Entities.Business();
                Field field = new Field();

                foreach (BasicProjectDTO item in listDTO)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    if (item.approvedDate != null)
                    {
                        item.approvedDate = await _validationService.FormatDateOutput(item.approvedDate);
                    }
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    business = await _businessRepository.GetBusinessById(Guid.Parse(item.businessId));
                    item.businessName = business.Name;
                    item.businessImage = business.Image;

                    field = await _fieldRepository.GetFieldById(Guid.Parse(item.fieldId));
                    item.fieldName = field.Name;
                    item.fieldDescription = field.Description;
                    
                    item.tagList = await _projectTagService.GetProjectTagList(item);

                    result.listOfProject.Add(item);
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
                    projectDTO.business.createDate = await _validationService.FormatDateOutput(projectDTO.business.createDate);
                    projectDTO.business.updateDate = await _validationService.FormatDateOutput(projectDTO.business.updateDate);
                    projectDTO.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(projectDTO.business.id)));
                    projectDTO.business.manager.createDate = await _validationService.FormatDateOutput(projectDTO.business.manager.createDate);
                    projectDTO.business.manager.updateDate = await _validationService.FormatDateOutput(projectDTO.business.manager.updateDate);
                    projectDTO.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(projectDTO.business.id)));
                    foreach (FieldDTO item in projectDTO.business.fieldList)
                    {
                        item.createDate = await _validationService.FormatDateOutput(item.createDate);
                        item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                    }
                }
                projectDTO.manager = _mapper.Map<ProjectManagerUserDTO>(await _userRepository.GetProjectManagerByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.manager.createDate = await _validationService.FormatDateOutput(projectDTO.manager.createDate);
                projectDTO.manager.updateDate = await _validationService.FormatDateOutput(projectDTO.manager.updateDate);
                projectDTO.field = _mapper.Map<FieldDTO>(await _fieldRepository.GetProjectFieldByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.field.createDate = await _validationService.FormatDateOutput(projectDTO.field.createDate);
                projectDTO.field.updateDate = await _validationService.FormatDateOutput(projectDTO.field.updateDate);
                projectDTO.area = _mapper.Map<AreaDTO>(await _areaRepository.GetAreaByProjectId(Guid.Parse(projectDTO.id)));
                projectDTO.projectEntity = new List<TypeProjectEntityDTO>();
                for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                {
                    TypeProjectEntityDTO typeProjectEntityDTO = new TypeProjectEntityDTO();
                    typeProjectEntityDTO.type = Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type);
                    typeProjectEntityDTO.typeItemList = _mapper.Map<List<ProjectComponentProjectEntityDTO>>(await _projectEntityRepository.GetProjectEntityByProjectIdAndType(Guid.Parse(projectDTO.id), Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)));
                    foreach (ProjectComponentProjectEntityDTO item in typeProjectEntityDTO.typeItemList)
                    { 
                        item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                    }
                    projectDTO.projectEntity.Add(typeProjectEntityDTO);
                }
                projectDTO.memberList = _mapper.Map<List<ProjectMemberUserDTO>>(await _userRepository.GetProjectMembers(Guid.Parse(projectDTO.id)));
                projectDTO.tagList = await _projectTagService.GetProjectTagList(_mapper.Map<BasicProjectDTO>(projectDTO));

                return projectDTO;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //
        public async Task<List<BusinessProjectDTO>> GetBusinessProjectsToAuthor(Guid businessId)
        {
            try
            {
                return await _projectRepository.GetBusinessProjectsToAuthor(businessId);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateProject(UpdateProjectDTO projectDTO, Guid projectId, ThisUserObj currentUser)
        {
            int result;
            bool dateChangedCheck = false;
            DateTime projectClosedDate;
            try
            {
                GetProjectDTO getProject = _mapper.Map<GetProjectDTO>(await _projectRepository.GetProjectById(projectId));
                if (getProject == null)
                    throw new NotFoundException("No Project Object Found!!!");
                Project project = new();

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
                else
                    projectDTO.investmentTargetCapital = getProject.investmentTargetCapital;

                if (projectDTO.sharedRevenue != 0)
                {
                    if (projectDTO.sharedRevenue <= 0 || projectDTO.sharedRevenue > 100)
                        throw new InvalidFieldException("sharedRevenue must be greater than 0 and less than 100!!!");
                }
                else
                    projectDTO.sharedRevenue = getProject.sharedRevenue;

                if (projectDTO.multiplier != 0)
                {
                    if (projectDTO.multiplier <= 0 || projectDTO.multiplier > 100)
                        throw new InvalidFieldException("multiplier must be greater than 0 and less than 100!!!");
                }
                else
                    projectDTO.multiplier = getProject.multiplier;

                if (projectDTO.duration != 0)
                {
                    if (projectDTO.duration <= 0)
                        throw new InvalidFieldException("duration must be greater than 0!!!");
                    if (projectDTO.duration == getProject.duration)
                        projectDTO.duration = 0;
                }

                if (projectDTO.numOfStage != 0)
                {
                    if (projectDTO.numOfStage <= 0)
                        throw new InvalidFieldException("numOfStage must be greater than 0!!!");
                    if (projectDTO.numOfStage == getProject.numOfStage)
                        projectDTO.numOfStage = 0;
                }

                if ((projectDTO.startDate != null || projectDTO.endDate != null) && (projectDTO.startDate == null || projectDTO.endDate == null))
                    throw new InvalidFieldException("startDate and endDate must be update at the same time!!!");

                if (projectDTO.startDate != null && projectDTO.endDate != null)
                {                   
                    if (!await _validationService.CheckDate((projectDTO.startDate)))
                        throw new InvalidFieldException("Invalid startDate!!!");

                    if (!await _validationService.CheckDate((projectDTO.endDate)))
                        throw new InvalidFieldException("Invalid endDate!!!");

                    if ((DateAndTime.DateDiff(DateInterval.Day, DateTime.ParseExact(projectDTO.startDate, "dd/MM/yyyy HH:mm:ss", null), DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null))) < 0)
                        throw new InvalidFieldException("startDate can not bigger than endDate!!!");

                    projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";
                    projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";

                    if (DateAndTime.DateDiff(DateInterval.Day,
                        DateTime.ParseExact(projectDTO.startDate, "dd/MM/yyyy HH:mm:ss", null),
                        DateTime.ParseExact(await _validationService.FormatDateOutput(getProject.startDate), "dd/MM/yyyy HH:mm:ss", null)) == 0)
                        projectDTO.startDate = null;

                    if (DateAndTime.DateDiff(DateInterval.Day,
                        DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null),
                        DateTime.ParseExact(await _validationService.FormatDateOutput(getProject.endDate), "dd/MM/yyyy HH:mm:ss", null)) == 0)
                        projectDTO.endDate = null;
                }

                //Update [Stage] and [PeriodRevenue]
                if (projectDTO.duration != 0 || projectDTO.numOfStage != 0 || (projectDTO.startDate != null && projectDTO.endDate != null))
                {
                    dateChangedCheck = true;

                    if (projectDTO.duration == 0)
                        projectDTO.duration = getProject.duration;

                    if (projectDTO.numOfStage == 0)
                        projectDTO.numOfStage = getProject.numOfStage;

                    if (projectDTO.startDate == null && projectDTO.endDate == null)
                    {
                        projectDTO.startDate = await _validationService.FormatDateOutput(getProject.startDate);
                        projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";
                        projectDTO.endDate = await _validationService.FormatDateOutput(getProject.endDate);
                        projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";
                    }
                    else
                    {
                        if (projectDTO.startDate == null)
                        {
                            projectDTO.startDate = await _validationService.FormatDateOutput(getProject.startDate);
                            projectDTO.startDate = projectDTO.startDate.Remove(projectDTO.startDate.Length - 8) + "00:00:00";
                        }
                        if (projectDTO.startDate == null)
                        {
                            projectDTO.endDate = await _validationService.FormatDateOutput(getProject.endDate);
                            projectDTO.endDate = projectDTO.endDate.Remove(projectDTO.endDate.Length - 8) + "23:59:59";
                        }
                    }

                    //Xóa [Stage] và [PeriodRevenue] cũ
                    await _periodRevenueRepository.DeletePeriodRevenueByProjectId(projectId);
                    await _stageRepository.DeleteStageByProjectId(projectId);

                    //Tạo lại [Stage] và [PeriodRevenue]

                    long totalDay = DateAndTime.DateDiff(DateInterval.Day, (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddDays(1), (DateTime.ParseExact(projectDTO.endDate, "dd/MM/yyyy HH:mm:ss", null)).AddMonths(projectDTO.duration)) + 1;
                    int daysPerStage = ((int)totalDay) / projectDTO.numOfStage;
                    int modDays = ((int)totalDay) - (daysPerStage * projectDTO.numOfStage);

                    //projectDTO.name = getProject.name;
                    projectDTO.startDate = await _validationService.FormatDateInput(projectDTO.startDate);
                    projectDTO.endDate = await _validationService.FormatDateInput(projectDTO.endDate);

                    project = _mapper.Map<Project>(projectDTO);
                   
                    Stage stage = new Stage();
                    PeriodRevenue periodRevenue = new PeriodRevenue();
                    string newStageId;
                    string newPeriodRevenueId;

                    stage.ProjectId = projectId;
                    stage.CreateBy = Guid.Parse(currentUser.userId);
                    stage.StartDate = DateTime.ParseExact(DateTime.Parse(project.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(project.EndDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "00:00:00", "dd/MM/yyyy HH:mm:ss", null).AddDays(11);
                    stage.EndDate = DateTime.ParseExact(DateTime.Parse(stage.StartDate.AddDays(daysPerStage - 1).ToString()).ToString("dd/MM/yyyy HH:mm:ss").Remove(DateTime.Parse(stage.StartDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss").Length - 8) + "23:59:59", "dd/MM/yyyy HH:mm:ss", null);

                    periodRevenue.ProjectId = projectId;
                    periodRevenue.CreateBy = Guid.Parse(currentUser.userId);

                    for (int i = 1; i <= projectDTO.numOfStage - 1; i++)
                    {
                        stage.Name = "Kỳ " + i;
                        newStageId = await _stageRepository.CreateStage(stage);
                        periodRevenue.StageId = Guid.Parse(newStageId);
                        newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                        stage.StartDate = stage.StartDate.AddDays(daysPerStage);
                        stage.EndDate = stage.EndDate.AddDays(daysPerStage);

                    }
                    stage.Name = "Kỳ " + projectDTO.numOfStage;
                    stage.EndDate = stage.EndDate.AddDays(modDays);
                    projectClosedDate = stage.EndDate;
                    newStageId = await _stageRepository.CreateStage(stage);
                    periodRevenue.StageId = Guid.Parse(newStageId);
                    newPeriodRevenueId = await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);                   
                }
                else
                {
                    projectDTO.duration = getProject.duration;
                    projectDTO.numOfStage = getProject.numOfStage;
                    projectDTO.startDate = getProject.startDate;
                    projectDTO.endDate = getProject.endDate;
                    project = _mapper.Map<Project>(projectDTO);

                    //
                    projectClosedDate = DateTimePicker.GetDateTimeByTimeZone();
                }                

                if (projectDTO.image != null)
                {
                    project.Image = await _fileUploadService.UploadImageToFirebaseProject(projectDTO.image, RoleDictionary.role.GetValueOrDefault("ADMIN"));
                }
                project.RemainingPayableAmount = project.InvestmentTargetCapital;
                //project.RemainingMaximumPayableAmount = (double)Math.Round(project.InvestmentTargetCapital * Math.Round(project.Multiplier, 1));
                project.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _projectRepository.UpdateProject(project, projectId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Project Object!");
                else
                {
                    if (dateChangedCheck)
                    {
                        //Update EXTENSION ProjectEntity
                        List<ProjectEntity> extensionList = await _projectEntityRepository.GetProjectEntityByProjectIdAndType(projectId, ProjectEntityEnum.EXTENSION.ToString());
                        ProjectEntity projectEntity = new ProjectEntity();
                        foreach (ProjectEntity item in extensionList)
                        {
                            if (item.Title.ToString().Equals("Ngày kết thúc gọi vốn"))
                            {
                                projectEntity.Content = (await _validationService.FormatDateOutput(projectDTO.endDate)).Remove(projectDTO.endDate.Length - 8);
                                projectEntity.UpdateBy = Guid.Parse(currentUser.userId);
                                await _projectEntityRepository.UpdateProjectEntity(projectEntity, item.Id);
                            }
                            else if (item.Title.ToString().Equals("Ngày dự đoán đóng dự án"))
                            {
                                projectEntity.Content = (await _validationService.FormatDateOutput(projectClosedDate.ToString())).Remove(projectDTO.endDate.Length - 8);
                                projectEntity.UpdateBy = Guid.Parse(currentUser.userId);
                                await _projectEntityRepository.UpdateProjectEntity(projectEntity, item.Id);
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE STATUS
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
                    if (project.Status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                    {
                        if (!status.Equals(ProjectStatusEnum.DENIED.ToString()) && !status.Equals(ProjectStatusEnum.WAITING_TO_PUBLISH.ToString()))
                            throw new InvalidFieldException("ADMIN can update Project's status from WAITING_FOR_APPROVAL to DENIED or WAITING_TO_PUBLISH!!!");
                    }
                    //else if (project.Status.Equals(ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString()))
                    //{
                    //    if (!status.Equals(ProjectStatusEnum.CLOSED.ToString()))
                    //        throw new InvalidFieldException("ADMIN can update Project's status from CALLING_TIME_IS_OVER to CLOSED!!!");
                    //}
                    else if (project.Status.Equals(ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString()))
                    {
                        if (!status.Equals(ProjectStatusEnum.ACTIVE.ToString()))
                            throw new InvalidFieldException("ADMIN can update Project's status from WAITING_TO_ACTIVATE to ACTIVE!!!");
                    }
                    else if (project.Status.Equals(ProjectStatusEnum.ACTIVE.ToString()))
                    {
                        if (!status.Equals(ProjectStatusEnum.CLOSED.ToString()))
                            throw new InvalidFieldException("ADMIN can update Project's status from ACTIVE to CLOSED!!!");

                        Stage lastStage = await _stageRepository.GetLastStageByProjectId(project.Id);
                        if (DateTime.Compare(DateTimePicker.GetDateTimeByTimeZone(), lastStage.EndDate.AddDays(3)) < 0)
                            throw new InvalidFieldException("You can not close this Project because all Stages have not DONE yet!!!");

                        if (project.PaidAmount < project.InvestmentTargetCapital)
                            throw new InvalidFieldException("You can not close this Project because the debt has not been paid in full!!!");
                    }
                    else
                        throw new InvalidFieldException("ADMIN can update if Project's status is WAITING_FOR_APPROVAL or CALLING_TIME_IS_OVER or WAITING_TO_ACTIVATE or ACTIVE!!!");
                }
                else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    if (project.Status.Equals(ProjectStatusEnum.DRAFT.ToString()))
                    {
                        if (!status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                            throw new InvalidFieldException("PROJECT_MANAGER can update Project's status from DRAFT to WAITING_FOR_APPROVAL!!!");
                    }
                    else if (project.Status.Equals(ProjectStatusEnum.DENIED.ToString()))
                    {
                        if (!status.Equals(ProjectStatusEnum.DRAFT.ToString()))
                            throw new InvalidFieldException("PROJECT_MANAGER can update Project's status from DENIED to DRAFT!!!");
                    }
                    else
                        throw new InvalidFieldException("PROJECT_MANAGER can update if Project's status is DRAFT or DENIED!!!");
                }

                result = status.Equals(ProjectStatusEnum.WAITING_TO_PUBLISH.ToString()) 
                    ? await _projectRepository.ApproveProject(projectId, Guid.Parse(currentUser.userId)) 
                    : await _projectRepository.UpdateProjectStatus(projectId, status, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update Project status!");
                else
                {
                    if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                    {
                        if (status.Equals(ProjectStatusEnum.WAITING_TO_PUBLISH.ToString()))
                        {

                            //Tạo Schedule chuyển status WAITING_TO_PUBLISH to CALLING_FOR_INVESTMENT
                            _backgroundJobClient.Schedule<ProjectService>(
                                projectService => projectService
                                .UpdateProjectStatusByHangfire(projectId, currentUser), TimeSpan.FromTicks(project.StartDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));

                            //Tạo Schedule chuyển status CALLING_FOR_INVESTMENT to CALLING_TIME_IS_OVER or WAITING_TO_ACTIVATE
                            _backgroundJobClient.Schedule<ProjectService>(
                                projectService => projectService
                                .UpdateProjectStatusByHangfire(projectId, currentUser), TimeSpan.FromTicks(project.EndDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));

                            ////Tạo Schedule chuyển status CALLING_FOR_INVESTMENT to WAITING_TO_ACTIVATE
                            //_backgroundJobClient.Schedule<ProjectService>(
                            //    projectService => projectService
                            //    .UpdateProjectStatusByHangfire(projectId, currentUser), TimeSpan.FromTicks(project.EndDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));
                            NotificationDetailDTO notification = new()
                            {
                                Title = "Dự án " + project.Name + " của bạn đã được duyệt và đang chờ đến ngày gọi vốn và sẽ được công bố/ công khai khi đến ngày.",
                                EntityId = projectId.ToString(),
                                Image = project.Image
                            };                        
                            await DistributedCacheExtensions.UpdateNotification(_cache, project.CreateBy.ToString(), notification);
                        }
                        else if (status.Equals(ProjectStatusEnum.ACTIVE.ToString()))
                        {
                            InvestorWallet investorWallet = new();
                            WalletTransaction walletTransaction = new();
                            NotificationDetailDTO notiForInvestor = new()
                            {
                                Title = "Dự án " + project.Name + " mà bạn góp vốn đã được đưa vào hoạt động.",
                                EntityId = projectId.ToString(),
                                Image = project.Image
                            };
                            
                            List<Investment> investmentList = await _investmentRepository
                                .GetAllInvestments(0, 0, null, null, projectId.ToString(), null, TransactionStatusEnum.SUCCESS.ToString(), Guid.Parse(currentUser.roleId));
                            ProjectWallet projectWallet = await _projectWalletRepository
                                .GetProjectWalletByProjectManagerIdAndType(project.ManagerId, WalletTypeEnum.P3.ToString(), projectId);
                            projectWallet.UpdateBy = Guid.Parse(currentUser.userId);

                            foreach (Investment item in investmentList)
                            {
                                //Subtract I3 balance
                                investorWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.InvestorId, WalletTypeEnum.I3.ToString());
                                investorWallet.Balance = -item.TotalPrice;
                                investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                                await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                                //Create CASH_OUT WalletTransaction from I3 to P3
                                walletTransaction = new WalletTransaction
                                {
                                    Amount = item.TotalPrice,
                                    Fee = 0,
                                    Description = "Transfer money from I3 wallet to P3 wallet to prepare for activation",
                                    FromWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.InvestorId, WalletTypeEnum.I3.ToString())).Id,
                                    CreateBy = Guid.Parse(currentUser.userId),
                                    Type = WalletTransactionTypeEnum.CASH_OUT.ToString()

                                };
                                walletTransaction.InvestorWalletId = walletTransaction.FromWalletId;
                                walletTransaction.ToWalletId = (await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(project.ManagerId, WalletTypeEnum.P3.ToString(), projectId)).Id;
                                walletTransaction.ProjectWalletId = walletTransaction.ToWalletId;
                                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                                //Add P3 balance
                                projectWallet.Balance = item.TotalPrice;
                                await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                                //Create CASH_IN WalletTransaction from I3 to P3
                                walletTransaction.Description = "Receive money from I3 wallet to P3 wallet to prepare for activation";
                                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                                await DistributedCacheExtensions.UpdateNotification(_cache, investorWallet.CreateBy.ToString(), notiForInvestor);

                            }

                            NotificationDetailDTO notification = new()
                            {
                                Title = "Dự án " + project.Name + " của bạn đã gọi vốn thành công và đang hoạt động.",
                                EntityId = projectId.ToString(),
                                Image = project.Image
                            };
                            await DistributedCacheExtensions.UpdateNotification(_cache, project.CreateBy.ToString(), notification);

                            //Create DailyReports
                            DailyReport dailyReport = new()
                            {
                                CreateDate = DateTimePicker.GetDateTimeByTimeZone(),
                                CreateBy = Guid.Parse(currentUser.userId)
                            };
                            List<Stage> stageList = await _stageRepository.GetAllStagesByProjectId(projectId, 0, 0, null);
                            int numOfReport;
                            foreach (Stage stage in stageList)
                            {
                                dailyReport.StageId = stage.Id;
                                dailyReport.ReportDate = stage.StartDate;

                                numOfReport = (int)DateAndTime.DateDiff(DateInterval.Day, stage.StartDate, stage.EndDate) + 1;

                                await _dailyReportRepository.CreateDailyReports(dailyReport, numOfReport);
                            }

                            Stage lastStage = await _stageRepository.GetLastStageByProjectId(projectId);
                            _backgroundJobClient.Schedule<ProjectService>(
                                projectService => projectService
                                .CreateRepaymentStageCheck(projectId, currentUser), TimeSpan.FromTicks(lastStage.EndDate.AddDays(3).Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));
                        }

                    }
                    else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    {
                        if (status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                        {
                            //Tạo Schedule chuyển status WAITING_FOR_APPROVAL to DENIED
                            _backgroundJobClient.Schedule<ProjectService>(
                                projectService => projectService
                                .UpdateProjectStatusByHangfire(projectId, currentUser), TimeSpan.FromTicks(project.StartDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET INVESTED PROJECTS
        public async Task<AllInvestedProjectDTO> GetInvestedProjects(int pageIndex, int pageSize, ThisUserObj currentUser)
        {
            AllInvestedProjectDTO result = new AllInvestedProjectDTO();
            result.listOfProject = new List<InvestedProjectDTO>();
            List<Project> listEntity = new List<Project>();

            try
            {
                result.numOfProject = await _projectRepository.CountInvestedProjects(Guid.Parse(currentUser.investorId));
                listEntity = await _projectRepository.GetInvestedProjects(pageIndex, pageSize, Guid.Parse(currentUser.investorId));

                List<InvestedProjectDTO> listDTO = _mapper.Map<List<InvestedProjectDTO>>(listEntity);

                foreach (InvestedProjectDTO item in listDTO)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    if (item.approvedDate != null)
                    {
                        item.approvedDate = await _validationService.FormatDateOutput(item.approvedDate);
                    }
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    item.investedAmount = await _paymentRepository.GetInvestedAmountForInvestorByProjectId(Guid.Parse(item.id), Guid.Parse(currentUser.userId));
                    item.receivedAmount = await _paymentRepository.GetReceivedAmountForInvestorByProjectId(Guid.Parse(item.id), Guid.Parse(currentUser.userId));
                    item.lastestInvestmentDate = await _validationService.FormatDateOutput(await _paymentRepository.GetLastestInvestmentDateForInvestorByProjectId(Guid.Parse(item.id), Guid.Parse(currentUser.userId)));

                    result.listOfProject.Add(item);
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<IntegrateInfo> GetIntegrateInfoByUserEmail(string projectId)
        {
            try
            {
                IntegrateInfo info = await _projectRepository.GetIntegrateInfoByProjectId(Guid.Parse(projectId));

                return info;
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GetProjectNameForContractById(string projectId)
        {
            try
            {
                string info = await _projectRepository.GetProjectNameForContractById(Guid.Parse(projectId));

                return info;
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }


        //UPDATE STATUS BY HANGFIRE
        public async Task<int> UpdateProjectStatusByHangfire(Guid projectId, ThisUserObj currentUser)
        {
            try
            {
                if (!await _validationService.CheckExistenceId("Project", projectId))
                    throw new NotFoundException("This projectId is not existed!!!");

                NotificationDetailDTO notification = new();

                Project project = await _projectRepository.GetProjectById(projectId);
                if (project.Status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                {
                    if (project.StartDate <= DateTimePicker.GetDateTimeByTimeZone() && project.ApprovedBy == null && project.ApprovedDate == null)
                    {
                        var result = await _projectRepository.UpdateProjectStatus(projectId, ProjectStatusEnum.DENIED.ToString(), Guid.Parse(currentUser.userId));
                        notification.Title = "Dự án "+project.Name+" của bạn đã bị từ chối vì ví do quá hạn.";
                        notification.EntityId = projectId.ToString();
                        notification.Image = project.Image;
                        await DistributedCacheExtensions.UpdateNotification(_cache, currentUser.userId, notification);
                        return result;
                    }
                        
                }
                else if (project.Status.Equals(ProjectStatusEnum.WAITING_TO_PUBLISH.ToString()))
                {
                    if (project.StartDate <= DateTimePicker.GetDateTimeByTimeZone())
                    {
                        var result = await _projectRepository.UpdateProjectStatus(projectId, ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), Guid.Parse(currentUser.userId));
                        notification.Title = "Dự án " + project.Name + " của bạn đã được chấp thuận và đang chờ được góp vốn.";
                        notification.EntityId = projectId.ToString();
                        notification.Image = project.Image;
                        await DistributedCacheExtensions.UpdateNotification(_cache, currentUser.userId, notification);
                        return result;
                    }
                        
                }
                else if (project.Status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString()))
                {
                    if (project.EndDate <= DateTimePicker.GetDateTimeByTimeZone() && project.InvestedCapital < project.InvestmentTargetCapital)
                    {
                        var result = await _projectRepository.UpdateProjectStatus(projectId, ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString(), Guid.Parse(currentUser.userId));
                        notification.Title = "Dự án " + project.Name + " của bạn đã hết thời gian gọi vốn.";
                        notification.EntityId = projectId.ToString();
                        notification.Image = project.Image;
                        await DistributedCacheExtensions.UpdateNotification(_cache, currentUser.userId, notification);

                        //REFUND
                        //Chuyển lại tiền cho Investor
                        List<Investment> investmentList = await _investmentRepository.GetAllInvestments(0, 0, null, null, project.Id.ToString(), null, TransactionStatusEnum.SUCCESS.ToString(), Guid.Parse(currentUser.roleId));
                        List<PaidInvestorDTO> paidInvestorList = new List<PaidInvestorDTO>();
                        PaidInvestorDTO paidInvestor = new PaidInvestorDTO();
                        InvestorWallet investorWallet = new InvestorWallet();
                        WalletTransaction walletTransaction = new WalletTransaction();

                        foreach (Investment item in investmentList)
                        {
                            if (paidInvestorList.Find(x => x.investorId.Equals(item.InvestorId)) == null)
                                paidInvestorList.Add(new PaidInvestorDTO(item.InvestorId, (double)item.TotalPrice));
                            else
                            {
                                var foundInvestor = paidInvestorList.ToDictionary(x => x.investorId);
                                paidInvestor = paidInvestorList.Find(x => x.investorId.Equals(item.InvestorId));
                                if (foundInvestor.TryGetValue(paidInvestor.investorId, out paidInvestor))
                                    paidInvestor.amount = paidInvestor.amount + (double)item.TotalPrice;
                            }
                        }                       

                        foreach (PaidInvestorDTO item in paidInvestorList)
                        {
                            //Subtract I3 balance
                            investorWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I3.ToString());
                            investorWallet.Balance = -item.amount;
                            investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                            await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                            //Create CASH_OUT WalletTransaction from I3 to I2
                            walletTransaction = new WalletTransaction();
                            walletTransaction.Amount = item.amount;
                            walletTransaction.Fee = 0;
                            walletTransaction.Description = "Transfer money from I3 wallet to I2 wallet due to unsuccessful project calling for investment";
                            walletTransaction.FromWalletId = investorWallet.Id;
                            walletTransaction.ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I2.ToString())).Id;
                            walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                            walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                            await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                            //Add I2 balance
                            investorWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I2.ToString());
                            investorWallet.Balance = item.amount;
                            investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                            await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                            //Create CASH_IN WalletTransaction from I3 to I2
                            walletTransaction.Description = "Receive money from I3 wallet to I2 wallet due to unsuccessful project calling for investment";
                            walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                            await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                            //Notification
                            notification.Title = "Nhận lại tiền từ dự án " + project.Name + " do gọi vốn không thành công.";
                            notification.EntityId = projectId.ToString();
                            notification.Image = project.Image;
                            await DistributedCacheExtensions.UpdateNotification(_cache, (await _userRepository.GetUserByInvestorId(item.investorId)).Id.ToString(), notification);
                        }
                        return result;
                    }
                        
                    else if (project.EndDate <= DateTimePicker.GetDateTimeByTimeZone() && project.InvestedCapital == project.InvestmentTargetCapital)
                    {
                        var result = await _projectRepository.UpdateProjectStatus(projectId, ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString(), Guid.Parse(currentUser.userId));
                        notification.Title = "Dự án " + project.Name + " của bạn đã gọi vốn thành công và đang chờ để triển khai.";
                        notification.EntityId = projectId.ToString();
                        notification.Image = project.Image;
                        await DistributedCacheExtensions.UpdateNotification(_cache, currentUser.userId, notification);
                        return result;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<InvestedProjectDetailWithInvestment> GetInvestedProjectDetail(string projectId, string investorId)
        {
            try
            {
                Guid project = Guid.Parse(projectId);
                Guid investor = Guid.Parse(investorId);

                InvestedProjectDetail detail = await _projectRepository.GetInvestedProjectDetail(project, investor);
                List<InvestedRecord> investedRecords = await _investmentRepository.GetInvestmentRecord(project, investor);


                InvestedProjectDetailWithInvestment detailWithInvestment = new()
                {
                    ProjectImage = detail.ProjectImage,
                    ProjectName = detail.ProjectName,
                    ProjectStatus = detail.ProjectStatus,
                    ExpectedReturn = detail.ExpectedReturn,
                    ReturnedAmount = await _projectRepository.GetReturnedDeptOfOneInvestor(project,investor),
                    InvestedAmount = detail.InvestedAmount,
                    NumOfStage = detail.NumOfStage,
                    InvestmentRecords = investedRecords
                };
                detailWithInvestment.MustPaidDept = detail.InvestedAmount - detailWithInvestment.ReturnedAmount;
                detailWithInvestment.ProfitableDebt = detail.ExpectedReturn - detailWithInvestment.ReturnedAmount;

                return detailWithInvestment;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CreateRepaymentStageCheck(Guid projectId, ThisUserObj currentUser)
        {
            try
            {
                Project project = await _projectRepository.GetProjectById(projectId);
                Stage lastStage = await _stageRepository.GetLastStageByProjectId(projectId);
                if (DateTime.Compare(DateTimePicker.GetDateTimeByTimeZone(), lastStage.EndDate.AddDays(3)) > 0 && project.PaidAmount < project.InvestmentTargetCapital)
                {
                    await _stageService.CreateRepaymentStage(projectId, currentUser);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
