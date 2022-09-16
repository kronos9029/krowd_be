using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public StageService(IStageRepository stageRepository, IPeriodRevenueRepository periodRevenueRepository, IProjectRepository projectRepository, IUserRepository userRepository, IValidationService validationService, IMapper mapper)
        {
            _stageRepository = stageRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _validationService = validationService;
            _mapper = mapper;
        }


        //DELETE
        public async Task<int> DeleteStageById(Guid stageId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckUUIDFormat(stageId.ToString()))
                    throw new InvalidFieldException("Invalid stageId!!!");

                Stage stage = await _stageRepository.GetStageById(stageId);
                if (stage == null)
                    throw new NotFoundException("No Stage Object Found!");

                result = await _stageRepository.DeleteStageById(stageId);
                if (result == 0)
                    throw new DeleteObjectException("Can Not Delete Stage Object!");
                else
                {
                    await _periodRevenueRepository.DeletePeriodRevenueByStageId(stageId);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<AllStageDTO> GetAllStagesByProjectId(Guid projectId, ThisUserObj currentUser)
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

                AllStageDTO result = new AllStageDTO();
                result.listOfStage = new List<GetStageDTO>();
                PeriodRevenue periodRevenue = new PeriodRevenue();

                List<Stage> stageList = await _stageRepository.GetAllStagesByProjectId(projectId);
                List<GetStageDTO> list = _mapper.Map<List<GetStageDTO>>(stageList);
                result.numOfStage = list.Count;              

                foreach (GetStageDTO item in list)
                {
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
                }

                result.listOfStage = list;

                return result;
            }
            catch (Exception e)
            {
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
                result.optimisticExpectedAmount = (periodRevenue.OptimisticExpectedAmount == null) ? 0 : (float)periodRevenue.OptimisticExpectedAmount;
                result.normalExpectedAmount = (periodRevenue.NormalExpectedAmount == null) ? 0 : (float)periodRevenue.NormalExpectedAmount;
                result.pessimisticExpectedAmount = (periodRevenue.PessimisticExpectedAmount == null) ? 0 : (float)periodRevenue.PessimisticExpectedAmount;
                result.optimisticExpectedRatio = (periodRevenue.OptimisticExpectedRatio == null) ? 0 : (float)periodRevenue.OptimisticExpectedRatio;
                result.normalExpectedRatio = (periodRevenue.NormalExpectedRatio == null) ? 0 : (float)periodRevenue.NormalExpectedRatio;
                result.pessimisticExpectedRatio = (periodRevenue.PessimisticExpectedRatio == null) ? 0 : (float)periodRevenue.PessimisticExpectedRatio;

                return result;
            }
            catch (Exception e)
            {
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

                if (!project.ManagerId.ToString().Equals(currentUser.userId))
                    throw new InvalidFieldException("This projectId is not belong to your Project!!!");

                ammountChart.chartName = "PeriodRevenue_Ammount_Chart";
                ammountChart.lineList = new List<StageLineDTO>();
                StageLineDTO OEA = new StageLineDTO();
                OEA.lineName = "Optimistic_Expected_Amount";
                OEA.data = new List<float>();
                StageLineDTO NEA = new StageLineDTO();
                NEA.lineName = "Normal_Expected_Amount";
                NEA.data = new List<float>();
                StageLineDTO PEA = new StageLineDTO();
                PEA.lineName = "Pessimistic_Expected_Amount";
                PEA.data = new List<float>();

                ratioChart.chartName = "PeriodRevenue_Ratio_Chart";
                ratioChart.lineList = new List<StageLineDTO>();
                StageLineDTO OER = new StageLineDTO();
                OER.lineName = "Optimistic_Expected_Ratio";
                OER.data = new List<float>();
                StageLineDTO NER = new StageLineDTO();
                NER.lineName = "Normal_Expected_Ratio";
                NER.data = new List<float>();
                StageLineDTO PER = new StageLineDTO();
                PER.lineName = "Pessimistic_Expected_Ratio";
                PER.data = new List<float>();

                List<Stage> stageList = await _stageRepository.GetAllStagesByProjectId(projectId);
                List<GetStageDTO> list = _mapper.Map<List<GetStageDTO>>(stageList);
                PeriodRevenue periodRevenue = new PeriodRevenue();

                foreach (GetStageDTO item in list)
                {
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

                if (stageDTO.startDate != null)
                {
                    if (!await _validationService.CheckDate((stageDTO.startDate)))
                        throw new InvalidFieldException("Invalid startDate!!!");

                    stageDTO.startDate = await _validationService.FormatDateInput(stageDTO.startDate);
                }

                if (stageDTO.endDate != null)
                {
                    if (!await _validationService.CheckDate((stageDTO.endDate)))
                        throw new InvalidFieldException("Invalid endDate!!!");

                    stageDTO.endDate = await _validationService.FormatDateInput(stageDTO.endDate);
                }

                if (stageDTO.pessimisticExpectedAmount != 0 
                    || stageDTO.normalExpectedAmount != 0
                    || stageDTO.optimisticExpectedAmount != 0
                    || stageDTO.pessimisticExpectedRatio != 0
                    || stageDTO.normalExpectedRatio != 0
                    || stageDTO.optimisticExpectedRatio != 0)
                {
                    float[] periodRevenueInput = { stageDTO.pessimisticExpectedAmount, stageDTO.normalExpectedAmount, stageDTO.optimisticExpectedAmount, stageDTO.pessimisticExpectedRatio, stageDTO.normalExpectedRatio, stageDTO.optimisticExpectedAmount };
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
