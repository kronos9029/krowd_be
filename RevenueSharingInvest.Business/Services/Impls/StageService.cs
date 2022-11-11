using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInvestmentRepository _investmentRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public StageService(
            IStageRepository stageRepository, 
            IPeriodRevenueRepository periodRevenueRepository, 
            IProjectRepository projectRepository, 
            IUserRepository userRepository,
            IInvestmentRepository investmentRepository,

            IValidationService validationService, 
            IMapper mapper)
        {
            _stageRepository = stageRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _investmentRepository = investmentRepository;

            _validationService = validationService;
            _mapper = mapper;
        }

        //GET ALL
        public async Task<AllStageDTO> GetAllStagesByProjectId(Guid projectId, int pageIndex, int pageSize, string status, ThisUserObj currentUser)
        {
            try
            {
                if (!await _validationService.CheckExistenceId("Project", projectId))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (!status.Equals("") && !Enum.IsDefined(typeof(StageStatusEnum), status)) throw new InvalidFieldException("status must be INACTIVE or UNDUE or DUE or DONE!!!");

                //Kiểm tra projectId có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(projectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    && !project.BusinessId.ToString().Equals(currentUser.businessId)) throw new NotFoundException("This projectId is not belong to your's Business!!!");

                bool isShowed = currentUser.roleId.Equals("") || (currentUser.roleId.Equals(currentUser.investorRoleId)
                    && await _investmentRepository.CountInvestmentByProjectAndInvestor(projectId, Guid.Parse(currentUser.investorId)) == 0) ? false : true;

                AllStageDTO result = new AllStageDTO();
                result.listOfStage = new List<GetStageDTO>();
                result.filterCount = new CountStageDTO();
                PeriodRevenue periodRevenue = new PeriodRevenue();

                List<Stage> stageList = await _stageRepository.GetAllStagesByProjectId(projectId, pageIndex, pageSize, status.Equals("") ? null : status);
                List<GetStageDTO> list = _mapper.Map<List<GetStageDTO>>(stageList);
                result.numOfStage = await _stageRepository.CountAllStagesByProjectId(projectId, status.Equals("") ? null : status);           

                foreach (GetStageDTO item in list)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(Guid.Parse(item.id));
                    if (periodRevenue == null)
                    {
                        periodRevenue = new PeriodRevenue();
                        periodRevenue.ProjectId = Guid.Parse(item.projectId);             
                        periodRevenue.StageId = Guid.Parse(item.id);
                        periodRevenue.Status = item.status;
                        periodRevenue.CreateBy = (item.createBy == null) ? null : Guid.Parse(item.projectId);
                        periodRevenue.UpdateBy = (item.updateBy == null) ? null : Guid.Parse(item.projectId);
                        await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                    }
                    item.actualAmount = periodRevenue.ActualAmount != null && isShowed ? (double)periodRevenue.ActualAmount : null;
                    item.sharedAmount = periodRevenue.SharedAmount != null && isShowed ? (double)periodRevenue.SharedAmount : null;
                    item.paidAmount = periodRevenue.PaidAmount != null && isShowed ? (double)periodRevenue.PaidAmount : null;
                    item.receivableAmount = item.sharedAmount == null ? null : null;
                    item.optimisticExpectedAmount = (periodRevenue.OptimisticExpectedAmount == null) ? 0 : (double)periodRevenue.OptimisticExpectedAmount;
                    item.normalExpectedAmount = (periodRevenue.NormalExpectedAmount == null) ? 0 : (double)periodRevenue.NormalExpectedAmount;
                    item.pessimisticExpectedAmount = (periodRevenue.PessimisticExpectedAmount == null) ? 0 : (double)periodRevenue.PessimisticExpectedAmount;
                    item.optimisticExpectedRatio = (periodRevenue.OptimisticExpectedRatio == null) ? 0 : (float)periodRevenue.OptimisticExpectedRatio;
                    item.normalExpectedRatio = (periodRevenue.NormalExpectedRatio == null) ? 0 : (float)periodRevenue.NormalExpectedRatio;
                    item.pessimisticExpectedRatio = (periodRevenue.PessimisticExpectedRatio == null) ? 0 : (float)periodRevenue.PessimisticExpectedRatio;
                }

                result.listOfStage = list;

                //Filter count
                List<Stage> filterCountStageList = await _stageRepository.GetAllStagesByProjectId(projectId, 0, 0, null);
                result.filterCount.all = filterCountStageList.Count;
                result.filterCount.inactive = filterCountStageList.FindAll(x => x.Status.Equals(StageStatusEnum.INACTIVE.ToString())).Count;
                result.filterCount.undue = filterCountStageList.FindAll(x => x.Status.Equals(StageStatusEnum.UNDUE.ToString())).Count;
                result.filterCount.due = filterCountStageList.FindAll(x => x.Status.Equals(StageStatusEnum.DUE.ToString())).Count;
                result.filterCount.done = filterCountStageList.FindAll(x => x.Status.Equals(StageStatusEnum.DONE.ToString())).Count;              

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetStageDTO> GetStageById(Guid stageId, ThisUserObj currentUser)
        {
            GetStageDTO result;
            try
            {              
                Stage stage = await _stageRepository.GetStageById(stageId);               
                if (stage == null)
                    throw new NotFoundException("No Stage Object Found!");

                bool isShowed = currentUser.roleId.Equals("") || (currentUser.roleId.Equals(currentUser.investorRoleId)
                    && await _investmentRepository.CountInvestmentByProjectAndInvestor(stage.ProjectId, Guid.Parse(currentUser.investorId)) == 0) ? false : true;

                //Kiểm tra projectId có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(stage.ProjectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    && !project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }
                //               
                result = _mapper.Map<GetStageDTO>(stage);

                PeriodRevenue periodRevenue = new PeriodRevenue();

                result.startDate = await _validationService.FormatDateOutput(result.startDate);
                result.endDate = await _validationService.FormatDateOutput(result.endDate);
                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);
                periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(Guid.Parse(result.id));
                if (periodRevenue == null)
                {
                    periodRevenue = new PeriodRevenue();
                    periodRevenue.ProjectId = Guid.Parse(result.projectId);
                    periodRevenue.StageId = Guid.Parse(result.id);
                    periodRevenue.Status = result.status;
                    periodRevenue.CreateBy = (result.createBy == null) ? null : Guid.Parse(result.projectId);
                    periodRevenue.UpdateBy = (result.updateBy == null) ? null : Guid.Parse(result.projectId);
                    await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                }
                result.actualAmount = periodRevenue.ActualAmount != null && isShowed ? (double)periodRevenue.ActualAmount : null;
                result.sharedAmount = periodRevenue.SharedAmount != null && isShowed ? (double)periodRevenue.SharedAmount : null;
                result.paidAmount = periodRevenue.PaidAmount != null && isShowed ? (double)periodRevenue.PaidAmount : null;
                result.optimisticExpectedAmount = (periodRevenue.OptimisticExpectedAmount == null) ? 0 : (double)periodRevenue.OptimisticExpectedAmount;
                result.normalExpectedAmount = (periodRevenue.NormalExpectedAmount == null) ? 0 : (double)periodRevenue.NormalExpectedAmount;
                result.pessimisticExpectedAmount = (periodRevenue.PessimisticExpectedAmount == null) ? 0 : (double)periodRevenue.PessimisticExpectedAmount;
                result.optimisticExpectedRatio = (periodRevenue.OptimisticExpectedRatio == null) ? 0 : (float)periodRevenue.OptimisticExpectedRatio;
                result.normalExpectedRatio = (periodRevenue.NormalExpectedRatio == null) ? 0 : (float)periodRevenue.NormalExpectedRatio;
                result.pessimisticExpectedRatio = (periodRevenue.PessimisticExpectedRatio == null) ? 0 : (float)periodRevenue.PessimisticExpectedRatio;

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }


        //GET CHART DATA
        public async Task<List<StageChartDTO>> GetStageChartByProjectId(Guid projectId, ThisUserObj currentUser)
        {
            List<StageChartDTO> result = new List<StageChartDTO>();
            StageChartDTO ammountChart = new StageChartDTO();
            StageChartDTO ratioChart = new StageChartDTO();
            try
            {
                Project project = await _projectRepository.GetProjectById(projectId);

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !project.BusinessId.ToString().Equals(currentUser.businessId))
                    throw new InvalidFieldException("The project has this projectId is not belong to your Business!!!");

                if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && !project.ManagerId.ToString().Equals(currentUser.userId))
                    throw new InvalidFieldException("This projectId is not belong to your Project!!!");

                ammountChart.chartName = "Biểu đồ số tiền doanh thu từng kỳ";
                ammountChart.lineList = new List<StageLineDTO>();
                StageLineDTO OEA = new StageLineDTO();
                OEA.name = "Số tiền mong đợi lạc quan";
                OEA.data = new List<double>();
                StageLineDTO NEA = new StageLineDTO();
                NEA.name = "Số tiền mong đợi";
                NEA.data = new List<double>();
                StageLineDTO PEA = new StageLineDTO();
                PEA.name = "Số tiền mong đợi bi quan";
                PEA.data = new List<double>();

                ratioChart.chartName = "Biểu đồ tỷ lệ doanh thu từng kỳ";
                ratioChart.lineList = new List<StageLineDTO>();
                StageLineDTO OER = new StageLineDTO();
                OER.name = "Tỷ lệ mong đợi lạc quan";
                OER.data = new List<double>();
                StageLineDTO NER = new StageLineDTO();
                NER.name = "Tỷ lệ mong đợi";
                NER.data = new List<double>();
                StageLineDTO PER = new StageLineDTO();
                PER.name = "Tỷ lệ mong đợi bi quan";
                PER.data = new List<double>();

                List<Stage> stageList = await _stageRepository.GetAllStagesByProjectId(projectId, 0, 0, null);
                List<GetStageDTO> list = _mapper.Map<List<GetStageDTO>>(stageList);
                PeriodRevenue periodRevenue = new PeriodRevenue();

                foreach (GetStageDTO item in list)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(Guid.Parse(item.id));
                    if (periodRevenue == null)
                    {
                        periodRevenue = new PeriodRevenue();
                        periodRevenue.ProjectId = Guid.Parse(item.projectId);
                        periodRevenue.StageId = Guid.Parse(item.id);
                        periodRevenue.Status = item.status;
                        periodRevenue.CreateBy = (item.createBy == null) ? null : Guid.Parse(item.projectId);
                        periodRevenue.UpdateBy = (item.updateBy == null) ? null : Guid.Parse(item.projectId);
                        await _periodRevenueRepository.CreatePeriodRevenue(periodRevenue);
                    }
                    item.optimisticExpectedAmount = (periodRevenue.OptimisticExpectedAmount == null) ? 0 : (float)periodRevenue.OptimisticExpectedAmount;
                    item.normalExpectedAmount = (periodRevenue.NormalExpectedAmount == null) ? 0 : (float)periodRevenue.NormalExpectedAmount;
                    item.pessimisticExpectedAmount = (periodRevenue.PessimisticExpectedAmount == null) ? 0 : (float)periodRevenue.PessimisticExpectedAmount;
                    item.optimisticExpectedRatio = (periodRevenue.OptimisticExpectedRatio == null) ? 0 : (float)periodRevenue.OptimisticExpectedRatio;
                    item.normalExpectedRatio = (periodRevenue.NormalExpectedRatio == null) ? 0 : (float)periodRevenue.NormalExpectedRatio;
                    item.pessimisticExpectedRatio = (periodRevenue.PessimisticExpectedRatio == null) ? 0 : (float)periodRevenue.PessimisticExpectedRatio;

                    OEA.data.Add(item.optimisticExpectedAmount);
                    NEA.data.Add(item.normalExpectedAmount);
                    PEA.data.Add(item.pessimisticExpectedAmount);
                    OER.data.Add(item.optimisticExpectedRatio);
                    NER.data.Add(item.normalExpectedRatio);
                    PER.data.Add(item.pessimisticExpectedRatio);
                }
                ammountChart.lineList.Add(OEA);
                ammountChart.lineList.Add(NEA);
                ammountChart.lineList.Add(PEA);
                ratioChart.lineList.Add(OER);
                ratioChart.lineList.Add(NER);
                ratioChart.lineList.Add(PER);
                result.Add(ammountChart);
                result.Add(ratioChart);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateStage(UpdateStageDTO stageDTO, Guid stageId, ThisUserObj currentUser)
        {
            int result;
            bool periodRevenueUpdateCheck = false;
            try
            {
                Stage stage = await _stageRepository.GetStageById(stageId);
                Project project = await _projectRepository.GetProjectById(stage.ProjectId);

                if (!project.ManagerId.ToString().Equals(currentUser.userId))
                    throw new InvalidFieldException("This stageId is not belong to your Project!!!");

                if (stageDTO.name != null)
                {
                    if (!await _validationService.CheckText(stageDTO.name))
                        throw new InvalidFieldException("Invalid name!!!");
                }      

                if (stageDTO.description != null && (stageDTO.description.Equals("string") || stageDTO.description.Length == 0))
                    stageDTO.description = null;

                if (stageDTO.pessimisticExpectedAmount != 0 
                    || stageDTO.normalExpectedAmount != 0
                    || stageDTO.optimisticExpectedAmount != 0
                    || stageDTO.pessimisticExpectedRatio != 0
                    || stageDTO.normalExpectedRatio != 0
                    || stageDTO.optimisticExpectedRatio != 0)
                {
                    double[] periodRevenueInput = { stageDTO.pessimisticExpectedAmount, stageDTO.normalExpectedAmount, stageDTO.optimisticExpectedAmount, stageDTO.pessimisticExpectedRatio, stageDTO.normalExpectedRatio, stageDTO.optimisticExpectedAmount };
                    foreach (float item in periodRevenueInput)
                    {
                        if (item == 0)
                            throw new InvalidFieldException("You must update all pessimisticExpectedAmount, normalExpectedAmount, optimisticExpectedAmount, pessimisticExpectedRatio, normalExpectedRatio, optimisticExpectedAmount at a same time!!!");
                        else
                            periodRevenueUpdateCheck = true;
                    }
                }

                Stage entity = _mapper.Map<Stage>(stageDTO);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _stageRepository.UpdateStage(entity, stageId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Stage Object!");
                else
                {
                    if (periodRevenueUpdateCheck)
                    {
                        PeriodRevenue periodRevenue = new PeriodRevenue();
                        periodRevenue.PessimisticExpectedAmount = stageDTO.pessimisticExpectedAmount;
                        periodRevenue.NormalExpectedAmount = stageDTO.normalExpectedAmount;
                        periodRevenue.OptimisticExpectedAmount = stageDTO.optimisticExpectedAmount;
                        periodRevenue.PessimisticExpectedRatio = stageDTO.pessimisticExpectedRatio;
                        periodRevenue.NormalExpectedRatio = stageDTO.normalExpectedRatio;
                        periodRevenue.OptimisticExpectedRatio = stageDTO.optimisticExpectedRatio;
                        periodRevenue.UpdateBy = Guid.Parse(currentUser.userId);

                        await _periodRevenueRepository.UpdatePeriodRevenueByStageId(periodRevenue, stageId);
                    }
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
