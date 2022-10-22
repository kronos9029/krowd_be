using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IDailyReportRepository _dailyReportRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        public BillService(
            IBillRepository billRepository,
            IProjectRepository projectRepository,
            IStageRepository stageRepository,
            IDailyReportRepository dailyReportRepository,

            IValidationService validationService,
            IMapper mapper)
        {
            _billRepository = billRepository;
            _projectRepository = projectRepository;
            _stageRepository = stageRepository;
            _dailyReportRepository = dailyReportRepository;

            _validationService = validationService;

            _mapper = mapper;
        }
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

                if (dailyReport.Amount != 0 && dailyReport.Status.Equals(DailyReportStatusEnum.REPORTED.ToString()))
                    throw new InvalidFieldException("You have reported for this day already!!!");
                else if (dailyReport.Amount != 0 && dailyReport.Status.Equals(DailyReportStatusEnum.UNDUE.ToString()))
                    throw new InvalidFieldException("You can report for this day soon!!!");

                for (int i = 0; i < bills.Bills.Count; i++)
                {
                    bills.Bills[i].DailyReportId = dailyReport.Id.ToString();
                    dailyReport.Amount += bills.Bills[i].Amount;
                }          

                var result = await _billRepository.BulkInsertInvoice(bills);

                await _dailyReportRepository.UpdateDailyReport(dailyReport);

                return _mapper.Map<DailyReportDTO>(await _dailyReportRepository.GetDailyReportById(dailyReport.Id));
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }
    }
}
