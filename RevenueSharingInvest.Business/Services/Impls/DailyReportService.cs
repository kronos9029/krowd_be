using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class DailyReportService : IDailyReportService
    {
        private readonly IDailyReportRepository _dailyReportRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public DailyReportService(IDailyReportRepository dailyReportRepository, IStageRepository stageRepository, IValidationService validationService, IMapper mapper)
        {
            _dailyReportRepository = dailyReportRepository;
            _stageRepository = stageRepository;

            _validationService = validationService;
            _mapper = mapper;
        }


        //GET ALL
        public async Task<AllDailyReportDTO> GetAllDailyReports(int pageIndex, int pageSize, Guid projectId, Guid? stageId, ThisUserObj currentUser)
        {
            AllDailyReportDTO result = new AllDailyReportDTO();
            result.listOfDailyReport = new List<DailyReportDTO>();
            try
            {
                if (!await _validationService.CheckExistenceId("Project", projectId))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (stageId != null && !await _validationService.CheckExistenceId("Stage", (Guid)stageId))
                    throw new NotFoundException("This stageId is not existed!!!");

                if (stageId != null)
                {
                    Stage stage = await _stageRepository.GetStageById((Guid)stageId);
                    if (!projectId.Equals(stage.ProjectId))
                        throw new InvalidFieldException("This stageId is not belong to the Project has this projectId!!!");
                }
                result.numOfDailyReport = await _dailyReportRepository.CountAllDailyReports(projectId.ToString(), stageId == null ? null : stageId.ToString(), currentUser.roleId);
                List<DailyReport> dailyReportList = await _dailyReportRepository.GetAllDailyReports(pageIndex, pageSize, projectId.ToString(), stageId == null ? null : stageId.ToString(), currentUser.roleId);
                List<DailyReportDTO> list = _mapper.Map<List<DailyReportDTO>>(dailyReportList);
                foreach (DailyReportDTO item in list)
                {
                    item.reportDate = await _validationService.FormatDateOutput(item.reportDate);
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = item.updateDate == null ? null : await _validationService.FormatDateOutput(item.updateDate);

                    result.listOfDailyReport.Add(item);
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY DATE
        public async Task<DailyReportDTO> GetDailyReportByDate(Guid id, string date, ThisUserObj currentUser)
        {
            DailyReportDTO result;
            try
            {
                DailyReport dto = await _dailyReportRepository.GetDailyReportByProjectIdAndDate(id, date);
                result = _mapper.Map<DailyReportDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No DailyReport Object Found!");

                result.reportDate = await _validationService.FormatDateOutput(result.reportDate);
                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = result.updateDate == null ? null : await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<DailyReportDTO> GetDailyReportById(Guid id, ThisUserObj currentUser)
        {
            DailyReportDTO result;
            try
            {
                DailyReport dto = await _dailyReportRepository.GetDailyReportById(id);
                result = _mapper.Map<DailyReportDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No DailyReport Object Found!");

                result.reportDate = await _validationService.FormatDateOutput(result.reportDate);
                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = result.updateDate == null ? null : await _validationService.FormatDateOutput(result.updateDate);

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
