using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class PeriodRevenueHistoryService : IPeriodRevenueHistoryService
    {
        private readonly IPeriodRevenueHistoryRepository _periodRevenueHistoryRepository;
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPackageRepository _packageRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PeriodRevenueHistoryService(
            IPeriodRevenueHistoryRepository periodRevenueHistoryRepository, 
            IPeriodRevenueRepository periodRevenueRepository, 
            IStageRepository stageRepository, 
            IProjectRepository projectRepository,
            IProjectWalletRepository projectWalletRepository,
            IInvestmentRepository investmentRepository,
            IPackageRepository packageRepository,


            IValidationService validationService, IMapper mapper)
        {
            _periodRevenueHistoryRepository = periodRevenueHistoryRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _stageRepository = stageRepository;
            _projectRepository = projectRepository;
            _projectWalletRepository = projectWalletRepository;
            _investmentRepository = investmentRepository;
            _packageRepository = packageRepository;

            _validationService = validationService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreatePeriodRevenueHistory(CreatePeriodRevenueHistoryDTO createPeriodRevenueHistoryDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                //if (createPeriodRevenueHistoryDTO.stageId == null || !await _validationService.CheckUUIDFormat(createPeriodRevenueHistoryDTO.stageId)) throw new InvalidFieldException("Invalid stageId!!!");

                Stage stage = await _stageRepository.GetStageById(Guid.Parse(createPeriodRevenueHistoryDTO.stageId));
                //if (stage == null) throw new InvalidFieldException("stageId is not existed!!!");
                //if (!stage.Status.Equals(StageStatusEnum.DUE.ToString())) throw new InvalidFieldException("The Stage's status is not DUE!!!");

                Project project = await _projectRepository.GetProjectById(stage.ProjectId);
                //if (!project.ManagerId.Equals(Guid.Parse(currentUser.userId))) throw new InvalidFieldException("This stageId is not belong to your Projects!!!");
                //if (createPeriodRevenueHistoryDTO.amount > project.RemainingMaximumPayableAmount) throw new InvalidFieldException("amount can not greater than Project's RemainingMaximumPayableAmount = " + project.RemainingMaximumPayableAmount +"!!!");

                //ProjectWallet projectWallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(Guid.Parse(currentUser.userId), WalletTypeEnum.P4.ToString(), project.Id);
                //if (projectWallet.Balance < createPeriodRevenueHistoryDTO.amount) throw new InvalidFieldException("You do not have enough money in PROJECT_PAYMENT_WALLET!!!");

                //PeriodRevenue periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(stage.Id);
                //if (periodRevenue == null) throw new NotFoundException("No PeriodRevenue Object Found!!!");

                //int numOfPeriodRevenueHistory = await _periodRevenueHistoryRepository.CountPeriodRevenueHistoryByPeriodRevenueId(periodRevenue.Id) + 1;

                //PeriodRevenueHistory periodRevenueHistory = new PeriodRevenueHistory();
                //periodRevenueHistory.Name = "Lần " + numOfPeriodRevenueHistory + " " + stage.Name;
                //periodRevenueHistory.PeriodRevenueId = periodRevenue.Id;
                //periodRevenueHistory.Amount = createPeriodRevenueHistoryDTO.amount;
                //periodRevenueHistory.Description = "Thanh toán cho " + stage.Name + " lần thứ " + numOfPeriodRevenueHistory;
                //periodRevenueHistory.CreateBy = Guid.Parse(currentUser.userId);

                //newId.id = await _periodRevenueHistoryRepository.CreatePeriodRevenueHistory(periodRevenueHistory);
                newId.id = "asdfa";
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create PeriodRevenueHistory Object!");
                else
                {
                    List<Package> packageList = await _packageRepository.GetAllPackagesByProjectId(project.Id);
                    List<PackagePercentDTO> packagePercentList = new List<PackagePercentDTO>();
                    PackagePercentDTO packagePercent;
                    foreach (Package package in packageList)
                    {
                        packagePercent = new PackagePercentDTO();
                        packagePercent.id = package.Id;
                        packagePercent.percent = (float)((package.Quantity - package.RemainingQuantity) * package.Price / project.InvestmentTargetCapital);
                        packagePercent.paidAmount = Math.Floor(packagePercent.percent * createPeriodRevenueHistoryDTO.amount);
                        packagePercentList.Add(packagePercent);                      
                    }

                    //Lấy các Investment
                    List<Investment> investmentList = await _investmentRepository.GetAllInvestments(0, 0, null, null, project.Id.ToString(), null, Guid.Parse(currentUser.roleId));

                    //Lấy các Investor
                }
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueHistoryDTO>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize)
        {
            try
            {
                List<PeriodRevenueHistory> periodRevenueHistoryList = await _periodRevenueHistoryRepository.GetAllPeriodRevenueHistories(pageIndex, pageSize);
                List<PeriodRevenueHistoryDTO> list = _mapper.Map<List<PeriodRevenueHistoryDTO>>(periodRevenueHistoryList);

                foreach (PeriodRevenueHistoryDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                }

                return list;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<PeriodRevenueHistoryDTO> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        {
            PeriodRevenueHistoryDTO result;
            try
            {
                PeriodRevenueHistory dto = await _periodRevenueHistoryRepository.GetPeriodRevenueHistoryById(periodRevenueHistoryId);
                result = _mapper.Map<PeriodRevenueHistoryDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No PeriodRevenueHistory Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);

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
