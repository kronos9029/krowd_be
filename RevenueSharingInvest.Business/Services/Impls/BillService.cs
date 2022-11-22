using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Data.Helpers.Logger;
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

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IDailyReportRepository _dailyReportRepository;
        private readonly IPeriodRevenueRepository _periodRevenueRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache  _cache;
        public BillService(
            IBillRepository billRepository,
            IProjectRepository projectRepository,
            IStageRepository stageRepository,
            IDailyReportRepository dailyReportRepository,
            IPeriodRevenueRepository periodRevenueRepository,

            IValidationService validationService,
            IMapper mapper,
            IDistributedCache cache)
        {
            _billRepository = billRepository;
            _projectRepository = projectRepository;
            _stageRepository = stageRepository;
            _dailyReportRepository = dailyReportRepository;
            _periodRevenueRepository = periodRevenueRepository;

            _validationService = validationService;

            _mapper = mapper;
            _cache = cache;
        }

        //CREATE
        public async Task<DailyReportDTO> BulkInsertBills(InsertBillDTO bills, string projectId, string date)
        {
            try
            {
                if (projectId == null || !await _validationService.CheckUUIDFormat(projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (date == null || !await _validationService.CheckDate(date))
                    throw new InvalidFieldException("Invalid date!!!");

                Project project = await _projectRepository.GetProjectById(Guid.Parse(projectId));
                DailyReport dailyReport = await _dailyReportRepository.GetDailyReportByProjectIdAndDate(Guid.Parse(projectId), date);
                if (dailyReport == null)
                    throw new InvalidFieldException("This date is not within the project revenue reporting periods!!!");

                if (dailyReport.Amount != 0 && dailyReport.Status.Equals(DailyReportStatusEnum.REPORTED.ToString()))
                    throw new InvalidFieldException("You have reported for this day already!!!");
                else if (dailyReport.Status.Equals(DailyReportStatusEnum.UNDUE.ToString()))
                    throw new InvalidFieldException("You can report for this day soon!!!");

                for (int i = 0; i < bills.bills.Count; i++)
                {
                    dailyReport.Amount += bills.bills[i].amount;
                }
                
                bills.dailyReportId = dailyReport.Id.ToString();

                await _billRepository.BulkInsertInvoice(bills);

                await _dailyReportRepository.UpdateDailyReport(dailyReport);

                Stage stage = await _stageRepository.GetStageById(dailyReport.StageId);
                //Check to create PeriodRevenue
                if (await _dailyReportRepository.CountNotReportedDailyReportsByStageId(stage.Id) == 0)
                {
                    PeriodRevenue periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(stage.Id);
                    periodRevenue.ActualAmount = new double();
                    List<DailyReport> dailyReportList = await _dailyReportRepository.GetAllDailyReportsByStageId(stage.Id);
                    foreach (DailyReport item in dailyReportList)
                    {
                        periodRevenue.ActualAmount += item.Amount;
                    }
                    periodRevenue.SharedAmount = periodRevenue.ActualAmount * project.SharedRevenue / 100;
                    await _periodRevenueRepository.UpdatePeriodRevenue(periodRevenue);
                }


                var result = _mapper.Map<DailyReportDTO>(await _dailyReportRepository.GetDailyReportById(dailyReport.Id));
                result.reportDate = result.reportDate == null ? null : await _validationService.FormatDateOutput(result.reportDate);
                result.createDate = result.createDate == null ? null : await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = result.updateDate == null ? null : await _validationService.FormatDateOutput(result.updateDate);

                PushNotification pushNotification = new()
                {
                    Title = "Có cập nhật mới về dự án "+project.Name,
                    Body = "Quản lý dự án " + project.Name + " vừa cập nhật doanh thu cho ngày " + dailyReport.ReportDate.ToString("dd/MM/yyyy"),
                    ImageUrl = project.Image
                };
                string topic = await FirebasePushNotification.SendPushNotificationToUpdateProjectTopics(projectId, pushNotification);
                NotificationDetailDTO notificationDetailDTO = new()
                {
                    Title = "Quản lý dự án " + project.Name + " vừa cập nhật doanh thu cho ngày " + dailyReport.ReportDate.ToString("dd/MM/yyyy"),
                    EntityId = projectId,
                    Image = project.Image
                };
                List<string> userIdList = await DeviceTokenCache.GetSubcribedUserFromTopic(_cache, topic);
                for(int i =0; i < userIdList.Count; i++)
                {
                    await NotificationCache.UpdateNotification(_cache, userIdList[i], notificationDetailDTO);
                }
                
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

        //GET ALL
        public async Task<AllBillDTO> GetAllBills(int pageIndex, int pageSize, Guid dailyReportId, ThisUserObj currentUser)
        {
            AllBillDTO result = new AllBillDTO();
            result.listOfBill = new List<GetBillDTO>();

            try
            {
                DailyReport dailyReport = await _dailyReportRepository.GetDailyReportById(dailyReportId);
                if (dailyReport == null)
                    throw new NotFoundException("No DailyReport Object Found!!!");
                Project project = await _projectRepository.GetProjectByDailyReportId(dailyReportId);

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !currentUser.businessId.Equals(project.BusinessId.ToString()))
                    throw new InvalidFieldException("This dailyReportId is not belong to your Business's Projects!!!");

                if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && !currentUser.userId.Equals(project.ManagerId.ToString()))
                    throw new InvalidFieldException("This dailyReportId is not belong to your Projects!!!");

                result.numOfBill = await _billRepository.CountAllBills(dailyReportId);
                result.listOfBill = _mapper.Map<List<GetBillDTO>>(await _billRepository.GetAllBills(pageIndex, pageSize, dailyReportId));

                foreach (GetBillDTO item in result.listOfBill)
                {
                    item.createDate = item.createDate == null ? null : await _validationService.FormatDateOutput(item.createDate);
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
        public async Task<GetBillDTO> GetBillById(Guid id, ThisUserObj currentUser)
        {
            GetBillDTO result;
            try
            {
                result = _mapper.Map<GetBillDTO>(await _billRepository.GetBillById(id));
                if (result == null)
                    throw new NotFoundException("No Bill Object Found!!!");
                DailyReport dailyReport = await _dailyReportRepository.GetDailyReportById(Guid.Parse(result.dailyReportId));
                if (dailyReport == null)
                    throw new NotFoundException("No DailyReport Object Found!!!");
                Project project = await _projectRepository.GetProjectByDailyReportId(dailyReport.Id);

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !currentUser.businessId.Equals(project.BusinessId.ToString()))
                    throw new InvalidFieldException("This billId is not belong to your Business's Projects!!!");

                if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && !currentUser.userId.Equals(project.ManagerId.ToString()))
                    throw new InvalidFieldException("This billId is not belong to your Projects!!!");

                result.createDate = result.createDate == null ? null : await _validationService.FormatDateOutput(result.createDate);

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
