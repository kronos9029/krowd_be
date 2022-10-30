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
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PeriodRevenueHistoryService(
            IPeriodRevenueHistoryRepository periodRevenueHistoryRepository, 
            IPeriodRevenueRepository periodRevenueRepository, 
            IStageRepository stageRepository, 
            IProjectRepository projectRepository,
            IProjectWalletRepository projectWalletRepository,
            IInvestorWalletRepository investorWalletRepository,
            IInvestmentRepository investmentRepository,
            IPackageRepository packageRepository,
            IWalletTransactionRepository walletTransactionRepository,

            IValidationService validationService, IMapper mapper)
        {
            _periodRevenueHistoryRepository = periodRevenueHistoryRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _stageRepository = stageRepository;
            _projectRepository = projectRepository;
            _projectWalletRepository = projectWalletRepository;
            _investorWalletRepository = investorWalletRepository;
            _investmentRepository = investmentRepository;
            _packageRepository = packageRepository;
            _walletTransactionRepository = walletTransactionRepository;

            _validationService = validationService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<PeriodRevenueHistoryDTO> CreatePeriodRevenueHistory(CreatePeriodRevenueHistoryDTO createPeriodRevenueHistoryDTO, ThisUserObj currentUser)
        {
            PeriodRevenueHistoryDTO result = new PeriodRevenueHistoryDTO();
            try
            {
                if (createPeriodRevenueHistoryDTO.stageId == null || !await _validationService.CheckUUIDFormat(createPeriodRevenueHistoryDTO.stageId)) throw new InvalidFieldException("Invalid stageId!!!");

                Stage stage = await _stageRepository.GetStageById(Guid.Parse(createPeriodRevenueHistoryDTO.stageId));
                if (stage == null) throw new InvalidFieldException("stageId is not existed!!!");
                if (!stage.Status.Equals(StageStatusEnum.DUE.ToString())) throw new InvalidFieldException("The Stage's status is not DUE!!!");

                Project project = await _projectRepository.GetProjectById(stage.ProjectId);
                if (!project.ManagerId.Equals(Guid.Parse(currentUser.userId))) throw new InvalidFieldException("This stageId is not belong to your Projects!!!");
                if (createPeriodRevenueHistoryDTO.amount > project.RemainingMaximumPayableAmount) throw new InvalidFieldException("amount can not greater than Project's RemainingMaximumPayableAmount = " + project.RemainingMaximumPayableAmount + "!!!");

                ProjectWallet projectWallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(Guid.Parse(currentUser.userId), WalletTypeEnum.P4.ToString(), project.Id);
                if (projectWallet.Balance < createPeriodRevenueHistoryDTO.amount) throw new InvalidFieldException("You do not have enough money in PROJECT_PAYMENT_WALLET!!!");

                PeriodRevenue periodRevenue = await _periodRevenueRepository.GetPeriodRevenueByStageId(stage.Id);
                if (periodRevenue == null) throw new NotFoundException("No PeriodRevenue Object Found!!!");

                int numOfPeriodRevenueHistory = await _periodRevenueHistoryRepository.CountPeriodRevenueHistoryByPeriodRevenueId(periodRevenue.Id) + 1;

                PeriodRevenueHistory periodRevenueHistory = new PeriodRevenueHistory();
                periodRevenueHistory.Name = "Lần " + numOfPeriodRevenueHistory + " " + stage.Name;
                periodRevenueHistory.PeriodRevenueId = periodRevenue.Id;
                periodRevenueHistory.Amount = createPeriodRevenueHistoryDTO.amount;
                periodRevenueHistory.Description = "Thanh toán cho " + stage.Name + " lần thứ " + numOfPeriodRevenueHistory;
                periodRevenueHistory.CreateBy = Guid.Parse(currentUser.userId);

                string newId = await _periodRevenueHistoryRepository.CreatePeriodRevenueHistory(periodRevenueHistory);
                if (newId.Equals(""))
                    throw new CreateObjectException("Can not create PeriodRevenueHistory Object!");
                else
                {
                    //Chuyển tiền cho Investor
                    periodRevenueHistory.Id = Guid.Parse(newId);

                    List<Investment> investmentList = await _investmentRepository.GetAllInvestments(0, 0, null, null, project.Id.ToString(), null, Guid.Parse(currentUser.roleId));
                    List<Investment> packageInvestmentList = new List<Investment>();

                    //***
                    List<PaidInvestorDTO> paidInvestorList = new List<PaidInvestorDTO>();

                    PaidInvestorDTO paidInvestor = new PaidInvestorDTO();
                    List<Package> packageList = await _packageRepository.GetAllPackagesByProjectId(project.Id);
                    List<PackagePercentDTO> packagePercentList = new List<PackagePercentDTO>();
                    PackagePercentDTO packagePercent;

                    foreach (Package package in packageList)
                    {
                        packagePercent = new PackagePercentDTO();
                        packagePercent.id = package.Id;
                        packagePercent.numOfPackage = package.Quantity - package.RemainingQuantity;
                        packagePercent.percent = (float)Math.Round(packagePercent.numOfPackage * package.Price / project.InvestmentTargetCapital, 2);
                        packagePercent.paidAmount = Math.Floor(packagePercent.percent * createPeriodRevenueHistoryDTO.amount);
                        packagePercent.paidPerInvestment = Math.Floor(packagePercent.paidAmount / packagePercent.numOfPackage);

                        packageInvestmentList = investmentList.FindAll(x => x.PackageId.Equals(packagePercent.id));

                        foreach (Investment investment in packageInvestmentList)
                        {
                            if (paidInvestorList.Find(x => x.investorId.Equals(investment.InvestorId)) == null)
                                paidInvestorList.Add(new PaidInvestorDTO(investment.InvestorId, Math.Floor((int)investment.Quantity * packagePercent.paidPerInvestment)));
                            else
                            {
                                var foundInvestor = paidInvestorList.ToDictionary(x => x.investorId);
                                paidInvestor = paidInvestorList.Find(x => x.investorId.Equals(investment.InvestorId));
                                if (foundInvestor.TryGetValue(paidInvestor.investorId, out paidInvestor)) 
                                    paidInvestor.amount = paidInvestor.amount + Math.Floor((int)investment.Quantity * packagePercent.paidPerInvestment);
                            }                    
                        }              
                    }

                    projectWallet = new ProjectWallet();
                    projectWallet.ProjectManagerId = project.ManagerId;
                    projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4"));
                    projectWallet.UpdateBy = Guid.Parse(currentUser.userId);

                    InvestorWallet investorWallet = new InvestorWallet();

                    WalletTransaction walletTransaction = new WalletTransaction();

                    foreach (PaidInvestorDTO item in paidInvestorList)
                    {
                        //Subtract P4 balance
                        projectWallet.Balance = -item.amount;
                        await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                        //Create CASH_OUT WalletTransaction from P4 to I4
                        walletTransaction = new WalletTransaction();
                        walletTransaction.Amount = item.amount;
                        walletTransaction.Fee = 0;
                        walletTransaction.Description = "Transfer money from I3 wallet to P3 wallet for stage payment";
                        walletTransaction.FromWalletId = (await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(project.ManagerId, WalletTypeEnum.P4.ToString(), project.Id)).Id;
                        walletTransaction.ProjectWalletId = walletTransaction.FromWalletId;
                        walletTransaction.ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I4.ToString())).Id;
                        walletTransaction.InvestorWalletId = walletTransaction.ToWalletId;
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                        walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Add I4 balance
                        investorWallet.InvestorId = item.investorId;
                        investorWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4"));
                        investorWallet.Balance = item.amount;
                        investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        //Create CASH_IN WalletTransaction from P4 to I4
                        walletTransaction.Description = "Receive money from I3 wallet to P3 wallet for stage payment";
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                    }

                    //Update PeriodRevenue
                    periodRevenue.PaidAmount = createPeriodRevenueHistoryDTO.amount;
                    periodRevenue.UpdateBy = Guid.Parse(currentUser.userId);
                    await _periodRevenueRepository.UpdatePeriodRevenueByPaidAmount(periodRevenue);

                    //Update Project Amounts
                    project.RemainingPayableAmount = project.RemainingPayableAmount - createPeriodRevenueHistoryDTO.amount;
                    project.RemainingMaximumPayableAmount = project.RemainingMaximumPayableAmount - createPeriodRevenueHistoryDTO.amount;
                    project.UpdateBy = Guid.Parse(currentUser.userId);
                    await _projectRepository.UpdateProjectRemainingAmount(project);
                }

                result = _mapper.Map<PeriodRevenueHistoryDTO>(periodRevenueHistory);

                return result;
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
