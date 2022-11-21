using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs.GetAllDTO;
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
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvestorRepository _investorRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;


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
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IInvestorRepository investorRepository,

            IValidationService validationService, 
            IMapper mapper,
            IDistributedCache cache)
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
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _investorRepository = investorRepository;

            _validationService = validationService;
            _mapper = mapper;
            _cache = cache;
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

                Project project = await _projectRepository.GetProjectById(stage.ProjectId);
                
                if (!project.ManagerId.Equals(Guid.Parse(currentUser.userId))) 
                    throw new InvalidFieldException("This stageId is not belong to your Projects!!!");
                
                if (!project.Status.Equals(ProjectStatusEnum.ACTIVE.ToString())) 
                    throw new InvalidFieldException("This Project is not ACTIVE!!!");
                
                if (stage.Status.Equals(StageStatusEnum.UNDUE.ToString())) 
                    throw new InvalidFieldException("You can not pay for this Stage now!!!");

                Stage lastStage = await _stageRepository.GetLastStageByProjectId(stage.ProjectId);
                if (DateTime.Compare(DateTimePicker.GetDateTimeByTimeZone(), lastStage.EndDate.AddDays(3)) > 0 && !stage.Id.Equals(lastStage.Id)) 
                    throw new InvalidFieldException("You can only pay for Repayment Stage from now!!!");

                if (createPeriodRevenueHistoryDTO.amount > (double)Math.Round(project.InvestmentTargetCapital * project.Multiplier) - project.PaidAmount) throw new InvalidFieldException("amount can not greater than " + (double)(Math.Round(project.InvestmentTargetCapital * project.Multiplier) - project.PaidAmount) + "!!!");

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

                    List<Investment> investmentList = await _investmentRepository.GetAllInvestments(0, 0, null, null, project.Id.ToString(), null, TransactionStatusEnum.SUCCESS.ToString(), Guid.Parse(currentUser.roleId));
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
                                    paidInvestor.amount += Math.Floor((int)investment.Quantity * packagePercent.paidPerInvestment);
                            }                    
                        }              
                    }

                    projectWallet.UpdateBy = Guid.Parse(currentUser.userId);

                    InvestorWallet investorWallet = new InvestorWallet();

                    WalletTransaction walletTransaction = new();
                    NotificationDetailDTO notificationForInvestor = new();
                    PushNotification pushNotification = new();

                    foreach (PaidInvestorDTO item in paidInvestorList)
                    {
                        //Subtract P4 balance
                        projectWallet.Balance = -item.amount;
                        await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                        //Create CASH_OUT WalletTransaction from P4 to I4
                        walletTransaction = new WalletTransaction();
                        walletTransaction.Amount = item.amount;
                        walletTransaction.Fee = 0;
                        walletTransaction.Description = "Transfer money from P4 wallet to I4 wallet for stage payment";
                        walletTransaction.FromWalletId = (await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(project.ManagerId, WalletTypeEnum.P4.ToString(), project.Id)).Id;
                        walletTransaction.ProjectWalletId = walletTransaction.FromWalletId;
                        walletTransaction.ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I4.ToString())).Id;
                        walletTransaction.InvestorWalletId = walletTransaction.ToWalletId;
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                        walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Add I4 balance
                        investorWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(item.investorId, WalletTypeEnum.I4.ToString());
                        investorWallet.Balance = item.amount;
                        investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        //Create CASH_IN WalletTransaction from P4 to I4
                        walletTransaction.Description = "Receive money from P4 wallet to I4 wallet for stage payment";
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Create Payment
                        Payment payment = new Payment();
                        payment.PeriodRevenueId = periodRevenue.Id;
                        payment.StageId = stage.Id;
                        payment.Amount = item.amount;
                        payment.Description = "Nhận tiền thanh toán lần " + numOfPeriodRevenueHistory + " của '" + stage.Name + "' từ dự án '" + project.Name + "'";
                        payment.Type = PaymentTypeEnum.PERIOD_REVENUE.ToString();
                        payment.FromId = Guid.Parse(currentUser.userId);
                        payment.ToId = (await _investorRepository.GetInvestorById(item.investorId)).UserId;
                        payment.CreateBy = Guid.Parse(currentUser.userId);
                        payment.Status = TransactionStatusEnum.SUCCESS.ToString();

                        string paymentId = await _paymentRepository.CreatePayment(payment);

                        //Notification cho investor
                        notificationForInvestor.Title = payment.Description;
                        notificationForInvestor.EntityId = paymentId;
                        notificationForInvestor.Image = project.Image;
                        await NotificationCache.UpdateNotification(_cache, payment.ToId.ToString(), notificationForInvestor);

                        pushNotification.Title = "Tiền về ví!!!";
                        pushNotification.Body = payment.Description;
                        pushNotification.ImageUrl = project.Image;
                        await FirebasePushNotification.SendPushNotificationToUpdateProjectTopics(project.Id.ToString(), pushNotification);
                    }

                    //Update PeriodRevenue
                    periodRevenue.PaidAmount = createPeriodRevenueHistoryDTO.amount;
                    periodRevenue.UpdateBy = Guid.Parse(currentUser.userId);
                    await _periodRevenueRepository.UpdatePeriodRevenueByPaidAmount(periodRevenue);

                    //Update Project Amounts
                    project.RemainingPayableAmount = project.RemainingPayableAmount - createPeriodRevenueHistoryDTO.amount;
                    project.PaidAmount = project.PaidAmount + createPeriodRevenueHistoryDTO.amount;
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
        public async Task<AllPeriodRevenueHistoryDTO> GetAllPeriodRevenueHistories(int pageIndex, int pageSize, Guid projectId, ThisUserObj currentUser)
        {
            try
            {
                AllPeriodRevenueHistoryDTO result = new AllPeriodRevenueHistoryDTO();
                result.listOfPeriodRevenueHistory = new List<PeriodRevenueHistoryDTO>();

                Project project = await _projectRepository.GetProjectById(projectId);

                if ((currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                    || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
                    && !project.BusinessId.Equals(Guid.Parse(currentUser.businessId)))
                    throw new InvalidFieldException("This project_id is not belong to your Business!!!");

                if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && !project.ManagerId.Equals(Guid.Parse(currentUser.userId)))
                    throw new InvalidFieldException("This is not your Project!!!");

                List<PeriodRevenueHistory> periodRevenueHistoryList = await _periodRevenueHistoryRepository.GetAllPeriodRevenueHistories(pageIndex, pageSize, projectId);
                result.listOfPeriodRevenueHistory = _mapper.Map<List<PeriodRevenueHistoryDTO>>(periodRevenueHistoryList);
                result.numOfPeriodRevenueHistory = await _periodRevenueHistoryRepository.CountAllPeriodRevenueHistories(projectId);

                foreach (PeriodRevenueHistoryDTO item in result.listOfPeriodRevenueHistory)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
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
