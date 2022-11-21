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
    public class InvestmentService : IInvestmentService
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public InvestmentService(
            IInvestmentRepository investmentRepository, 
            IInvestorRepository investorRepository, 
            IPackageRepository packageRepository, 
            IInvestorWalletRepository investorWalletRepository,
            IProjectWalletRepository projectWalletRepository, 
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IWalletTypeRepository walletTypeRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IValidationService validationService, 
            IMapper mapper,
            IDistributedCache cache)
        {
            _investorRepository = investorRepository;
            _investmentRepository = investmentRepository;
            _packageRepository = packageRepository;
            _investorWalletRepository = investorWalletRepository;
            _projectWalletRepository = projectWalletRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _walletTypeRepository = walletTypeRepository;
            _walletTransactionRepository = walletTransactionRepository;

            _validationService = validationService;
            _mapper = mapper;
            _cache = cache;
        }

        //CANCEL
        public async Task<int> CancelInvestment(Guid investmentId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                Investment investment = await _investmentRepository.GetInvestmentById(investmentId);

                if (!investment.InvestorId.Equals(Guid.Parse(currentUser.investorId))) throw new InvalidFieldException("This is not your Investment!!!");
                if (!investment.Status.Equals(TransactionStatusEnum.SUCCESS.ToString())) throw new UpdateObjectException("You can not cancel this Investment because its status is not 'SUCCESS'!!!");
                if (DateTime.Compare(DateTimePicker.GetDateTimeByTimeZone(), investment.CreateDate.Value.AddDays(1)) > 0) throw new UpdateObjectException("You can cancel an Investment within 24 hours only!!!");

                result = await _investmentRepository.CancelInvestment(investmentId, Guid.Parse(currentUser.userId));

                if (result != 0)
                {
                    //Subtract I3 balance
                    InvestorWallet investorWallet = await _investorWalletRepository
                        .GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString());
                    investorWallet.Balance = -investment.TotalPrice;
                    investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                    await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                    //Create CASH_OUT WalletTransaction from I3 to I2
                    WalletTransaction walletTransaction = new WalletTransaction();
                    walletTransaction.Amount = investment.TotalPrice;
                    walletTransaction.Fee = 0;
                    walletTransaction.Description = "Transfer money from I3 wallet to I2 wallet due to investment cancellation";
                    walletTransaction.FromWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString())).Id;
                    walletTransaction.ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString())).Id;
                    walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                    walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                    await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                    //Add I2 balance
                    investorWallet = await _investorWalletRepository
                        .GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString());
                    investorWallet.Balance = investment.TotalPrice;
                    investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                    await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                    //Create CASH_IN WalletTransaction from I3 to I2
                    walletTransaction.Description = "Receive money from I3 wallet to I2 wallet due to investment cancellation";
                    walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                    await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                    //Update Project Amount
                    await _projectRepository.UpdateProjectInvestedCapital(investment.ProjectId, (double)-investment.TotalPrice, Guid.Parse(currentUser.userId));
                }
                else throw new UpdateObjectException("Cancel failed!!!");

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<InvestmentPaymentDTO> CreateInvestment(CreateInvestmentDTO investmentDTO, ThisUserObj currentUser)
        {
            InvestmentPaymentDTO result = new();
            string investmentId = "";
            try
            {
                //Check information
                if (investmentDTO.projectId == null || !await _validationService.CheckUUIDFormat(investmentDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(investmentDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (investmentDTO.packageId == null || !await _validationService.CheckUUIDFormat(investmentDTO.packageId))
                    throw new InvalidFieldException("Invalid packageId!!!");

                if (!await _validationService.CheckExistenceId("Package", Guid.Parse(investmentDTO.packageId)))
                    throw new NotFoundException("This packageId is not existed!!!");

                if (investmentDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                Package package = await _packageRepository.GetPackageById(Guid.Parse(investmentDTO.packageId));

                Project project = await _projectRepository.GetProjectById(package.ProjectId);

                if ((double)(project.InvestedCapital + (package.Price * investmentDTO.quantity)) > project.InvestmentTargetCapital)
                    throw new InvalidFieldException("You can not invest because InvestedCapital after investment is greater than InvestmentTargetCapital!!!");

                if (investmentDTO.quantity > package.RemainingQuantity)
                    throw new InvalidFieldException("This package remainingQuantity is " + package.RemainingQuantity + "!!!");

                if ((await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString())).Balance < (double)(investmentDTO.quantity * package.Price))
                    throw new InvalidFieldException("You do not have enough money to invest!!!");

                //Create Investment
                Investment investment = _mapper.Map<Investment>(investmentDTO);
                investment.InvestorId = Guid.Parse(currentUser.investorId);
                investment.TotalPrice = investment.Quantity * package.Price;
                investment.Status = TransactionStatusEnum.WAITING.ToString();
                investment.CreateBy = Guid.Parse(currentUser.userId);
                investment.UpdateBy = Guid.Parse(currentUser.userId);

                investmentId = await _investmentRepository.CreateInvestment(investment);
                if (investmentId.Equals(""))
                {
                    //Create Payment
                    Payment payment = new Payment();
                    User projectManager = await _userRepository.GetProjectManagerByProjectId(package.ProjectId);
                    payment.InvestmentId = null;
                    payment.PackageId = package.Id;
                    payment.Amount = investment.TotalPrice;
                    payment.Description = "Đầu tư gói '" + package.Name + "' x" + investmentDTO.quantity + " của dự án '" + project.Name + "'";
                    payment.Type = PaymentTypeEnum.INVESTMENT.ToString();
                    payment.FromId = Guid.Parse(currentUser.userId);
                    payment.ToId = projectManager.Id;
                    payment.CreateBy = Guid.Parse(currentUser.userId);
                    payment.Status = TransactionStatusEnum.FAILED.ToString();

                    string paymentId = await _paymentRepository.CreatePayment(payment);

                    result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                    result.createDate = await _validationService.FormatDateOutput(result.createDate);
                    //Project project = await _projectRepository.GetProjectById(package.ProjectId);
                    result.projectId = project.Id.ToString();
                    result.projectName = project.Name;
                    result.packageId = package.Id.ToString();
                    result.packageName = package.Name;
                    result.investedQuantity = investmentDTO.quantity;
                    result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))).Name;
                    result.fee = "0%";

                    NotificationDetailDTO notification = new()
                    {
                        Title = "Đầu tư vào dự án " + project.Name + " không thành công, vui lòng thử lại hoặc liên hệ với Krowd Help Center.",
                        EntityId = project.Id.ToString(),
                        Image = project.Image
                    };
                    await NotificationCache.UpdateNotification(_cache, currentUser.userId, notification);
                }
                else
                {
                    //Create Payment
                    
                    User projectManager = await _userRepository.GetProjectManagerByProjectId(package.ProjectId);
                    Payment payment = new()
                    {
                        InvestmentId = Guid.Parse(investmentId),
                        PackageId = package.Id,
                        Amount = investment.TotalPrice,
                        Description = "Đầu tư gói '" + package.Name + "' x" + investmentDTO.quantity,
                        Type = PaymentTypeEnum.INVESTMENT.ToString(),
                        FromId = Guid.Parse(currentUser.userId),
                        ToId = projectManager.Id,
                        CreateBy = Guid.Parse(currentUser.userId),
                        Status = TransactionStatusEnum.SUCCESS.ToString()
                    };

                    string paymentId = await _paymentRepository.CreatePayment(payment);
                    if (paymentId != "")
                    {
                        //Update Investment Status
                        investment = new()
                        {
                            Id = Guid.Parse(investmentId),
                            UpdateBy = Guid.Parse(currentUser.userId),
                            Status = TransactionStatusEnum.SUCCESS.ToString()
                        };
                        await _investmentRepository.UpdateInvestmentStatus(investment);

                        //Update Package Quantity
                        await _packageRepository.UpdatePackageRemainingQuantity(package.Id, package.RemainingQuantity - investmentDTO.quantity, Guid.Parse(currentUser.userId));

                        //Update Project Amount
                        await _projectRepository.UpdateProjectInvestedCapital(package.ProjectId, (double)payment.Amount, Guid.Parse(currentUser.userId));

                        //Subtract I2 balance
                        InvestorWallet investorWallet = await _investorWalletRepository
                            .GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString());
                        investorWallet.Balance = -payment.Amount;
                        investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        //Create CASH_OUT WalletTransaction from I2 to I3
                        WalletTransaction walletTransaction = new()
                        {
                            PaymentId = Guid.Parse(paymentId),
                            Amount = payment.Amount,
                            Fee = 0,
                            Description = "Transfer money from I2 wallet to I3 wallet to invest",
                            FromWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString())).Id,
                            ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString())).Id,
                            Type = WalletTransactionTypeEnum.CASH_OUT.ToString(),
                            CreateBy = Guid.Parse(currentUser.userId)
                        };
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Add I3 balance
                        investorWallet = await _investorWalletRepository
                            .GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString());
                        investorWallet.Balance = payment.Amount;
                        investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        //Create CASH_IN WalletTransaction from I2 to I3
                        walletTransaction.Description = "Receive money from I2 wallet to I3 wallet to invest";
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Format Payment response
                        result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                        result.createDate = await _validationService.FormatDateOutput(result.createDate);
                        //Project project = await _projectRepository.GetProjectById(package.ProjectId);
                        result.projectId = project.Id.ToString();
                        result.projectName = project.Name;
                        result.packageId = package.Id.ToString();
                        result.packageName = package.Name;
                        result.investedQuantity = investmentDTO.quantity;
                        result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))).Name;
                        result.fee = walletTransaction.Fee + "%";

                        NotificationDetailDTO notification = new()
                        {
                            Title = "Đầu tư vào dự án " + project.Name + " thành công, từ giờ bạn sẽ nhận được những cập nhật mới nhất của dự án.",
                            EntityId = project.Id.ToString(),
                            Image = project.Image
                        };
                        await NotificationCache.UpdateNotification(_cache, currentUser.userId, notification);
                        notification.Title = currentUser.fullName+" vừa đầu tư vào dự án "+project.Name+" của bạn.";

                        await NotificationCache.UpdateNotification(_cache, projectManager.Id.ToString(), notification);
                        DeviceToken tokens = await DeviceTokenCache.GetAvailableDevice(_cache, currentUser.userId);
                        if (tokens.Tokens.Count > 0)
                            await FirebasePushNotification.SubcribeTokensToUpdateProjectTopics(_cache, tokens, project.Id.ToString(), currentUser.userId);
                    }
                    else
                    {
                        payment.Status = TransactionStatusEnum.FAILED.ToString();
                        paymentId = await _paymentRepository.CreatePayment(payment);
                        result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                        result.createDate = await _validationService.FormatDateOutput(result.createDate);
                        //Project project = await _projectRepository.GetProjectById(package.ProjectId);
                        result.projectId = project.Id.ToString();
                        result.projectName = project.Name;
                        result.packageId = package.Id.ToString();
                        result.packageName = package.Name;
                        result.investedQuantity = investmentDTO.quantity;
                        result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))).Name;
                        result.fee = "0%";

                        investment = new Investment();
                        investment.Id = Guid.Parse(investmentId);
                        investment.UpdateBy = Guid.Parse(currentUser.userId);
                        investment.Status = TransactionStatusEnum.FAILED.ToString();
                        await _investmentRepository.UpdateInvestmentStatus(investment);
                        NotificationDetailDTO notification = new()
                        {
                            Title = "Đầu tư vào dự án " + project.Name + " không thành công, vui lòng thử lại hoặc liên hệ với Krowd Help Center.",
                            EntityId = project.Id.ToString(),
                            Image = project.Image
                        };
                        await NotificationCache.UpdateNotification(_cache, currentUser.userId, notification);
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

        //GET ALL
        public async Task<AllInvestmentDTO> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, string status, ThisUserObj currentUser)
        {
            try
            {
                AllInvestmentDTO result = new AllInvestmentDTO();
                result.listOfInvestment = new List<GetInvestmentDTO>();
                result.filterCount = new CountInvestmentDTO();

                if (walletTypeId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(walletTypeId.ToString()))
                        throw new InvalidFieldException("Invalid walletTypeId!!!");

                    if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(walletTypeId)))
                        throw new NotFoundException("This walletTypeId is not existed!!!");

                    if (!walletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"), StringComparison.InvariantCultureIgnoreCase)
                        && !walletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new InvalidFieldException("walletTypeId must be the id of type I3 or I4 WalletType!!!");
                    }
                }
                if (businessId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(businessId.ToString()))
                        throw new InvalidFieldException("Invalid businessId!!!");

                    if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                        throw new NotFoundException("This businessId is not existed!!!");
                }
                if (projectId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(projectId.ToString()))
                        throw new InvalidFieldException("Invalid projectId!!!");

                    if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectId)))
                        throw new NotFoundException("This projectId is not existed!!!");
                }
                if (investorId != null)
                {
                    if (!await _validationService.CheckUUIDFormat(investorId.ToString()))
                        throw new InvalidFieldException("Invalid investorId!!!");

                    if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investorId)))
                        throw new NotFoundException("This investorId is not existed!!!");
                }
                if (status != null && !Enum.IsDefined(typeof(TransactionStatusEnum), status)) throw new InvalidFieldException("status must be WAITING or SUCCESS or FAILED or CANCELED!!!");
                
                if (currentUser.roleId.Equals(currentUser.adminRoleId, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (businessId != null && projectId != null)
                    {
                        Project project = await _projectRepository.GetProjectById(Guid.Parse(projectId));
                        if (!project.BusinessId.Equals(Guid.Parse(businessId))) throw new InvalidFieldException("This Project's projectId is not belong to this Business's businessId!!!");
                    }
                }
                else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId, StringComparison.InvariantCultureIgnoreCase) 
                    || currentUser.roleId.Equals(currentUser.projectManagerRoleId, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (currentUser.roleId.Equals(currentUser.projectManagerRoleId, StringComparison.InvariantCultureIgnoreCase) && projectId == null)
                        throw new InvalidFieldException("projectId can not be empty!!!");

                    if (businessId != null && !currentUser.businessId.Equals(businessId, StringComparison.InvariantCultureIgnoreCase))
                        throw new InvalidFieldException("This businessId is not match with your businessId!!!");

                    if (projectId != null)
                    {
                        Project project = await _projectRepository.GetProjectById(Guid.Parse(projectId));
                        if (currentUser.roleId.Equals(currentUser.businessManagerRoleId, StringComparison.InvariantCultureIgnoreCase) 
                            && !currentUser.businessId.Equals(project.BusinessId.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            throw new InvalidFieldException("This projectId is not belong to your Business!!!");

                        if (currentUser.roleId.Equals(currentUser.projectManagerRoleId, StringComparison.InvariantCultureIgnoreCase)
                                && !currentUser.userId.Equals(project.ManagerId.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            throw new InvalidFieldException("This projectId is not belong to your Project!!!");
                    }
                }
                else if (currentUser.roleId.Equals(currentUser.investorRoleId, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (investorId != null && !currentUser.investorId.Equals(investorId, StringComparison.InvariantCultureIgnoreCase))
                        throw new InvalidFieldException("This investorId is not match with your investorId!!!");
                    else
                        investorId = currentUser.investorId;
                }

                List<Investment> investmentList = await _investmentRepository.GetAllInvestments(pageIndex, pageSize, walletTypeId, businessId, projectId, investorId, status, Guid.Parse(currentUser.roleId));
                result.listOfInvestment = _mapper.Map<List<GetInvestmentDTO>>(investmentList);
                result.numOfInvestment = await _investmentRepository.CountAllInvestments(walletTypeId, businessId, projectId, investorId, status, Guid.Parse(currentUser.roleId));

                foreach (GetInvestmentDTO item in result.listOfInvestment)
                {
                    User user = await _userRepository.GetUserByInvestorId(Guid.Parse(item.investorId));
                    item.investorName = (user.FirstName == null ? "" : user.FirstName) + " " + (user.LastName == null ? "" : user.LastName);
                    item.investorImage = user.Image;
                    item.investorEmail = user.Email;

                    Package package = await _packageRepository.GetPackageById(Guid.Parse(item.packageId));
                    item.packageName = package.Name;
                    item.packagePrice = package.Price;

                    item.projectName = (await _projectRepository.GetProjectById(Guid.Parse(item.projectId))).Name;

                    item.createDate = item.createDate == null ? null : await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = item.updateDate == null ? null : await _validationService.FormatDateOutput(item.updateDate);
                }

                List<Investment> filterCountList = await _investmentRepository
                    .GetAllInvestments(0, 0, null, 
                    currentUser.roleId.Equals(currentUser.businessManagerRoleId) ? businessId : null,
                    currentUser.roleId.Equals(currentUser.projectManagerRoleId) ? projectId : null,
                    currentUser.roleId.Equals(currentUser.investorRoleId) ? investorId : null, 
                    null, Guid.Parse(currentUser.roleId));

                result.filterCount.all = filterCountList.Count;
                result.filterCount.waiting = filterCountList.FindAll(x => x.Status.Equals(TransactionStatusEnum.WAITING.ToString())).Count;
                result.filterCount.success = filterCountList.FindAll(x => x.Status.Equals(TransactionStatusEnum.SUCCESS.ToString())).Count;
                result.filterCount.failed = filterCountList.FindAll(x => x.Status.Equals(TransactionStatusEnum.FAILED.ToString())).Count;
                result.filterCount.canceled = filterCountList.FindAll(x => x.Status.Equals(TransactionStatusEnum.CANCELED.ToString())).Count;

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetInvestmentDTO> GetInvestmentById(Guid investmentId, ThisUserObj currentUser)
        {
            GetInvestmentDTO result;
            try
            {
                Investment investment = await _investmentRepository.GetInvestmentById(investmentId);
                if (investment == null)
                    throw new InvalidFieldException("No Investment Object Found!!!");

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) || currentUser.roleId.Equals(currentUser.businessManagerRoleId))
                {
                    Project project = await _projectRepository.GetProjectById(investment.ProjectId);

                    if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !project.BusinessId.ToString().Equals(currentUser.businessId))
                        throw new InvalidFieldException("This investmentId is not belong to your Business!!!");

                    if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && !project.ManagerId.ToString().Equals(currentUser.userId))
                        throw new InvalidFieldException("This investmentId is not belong to your Project!!!");
                }
                else if (currentUser.roleId.Equals(currentUser.investorRoleId))
                {
                    if (!investment.InvestorId.ToString().Equals(currentUser.investorId))
                        throw new InvalidFieldException("This investmentId is not belong to your Investment!!!");
                }

                result = _mapper.Map<GetInvestmentDTO>(investment);
                if (result == null)
                    throw new NotFoundException("No Investment Object Found!");

                User user = await _userRepository.GetUserByInvestorId(Guid.Parse(result.investorId));
                result.investorName = (user.FirstName == null ? "" : user.FirstName) + " " + (user.LastName == null ? "" : user.LastName);
                result.investorImage = user.Image;
                result.investorEmail = user.Email;

                Package package = await _packageRepository.GetPackageById(Guid.Parse(result.packageId));
                result.packageName = package.Name;
                result.packagePrice = package.Price;

                result.projectName = (await _projectRepository.GetProjectById(Guid.Parse(result.projectId))).Name;

                result.createDate = result.createDate == null ? null : await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = result.updateDate == null ? null : await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET FOR AUTHOR
        public async Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId)
        {
            try
            {
                return await _investmentRepository.GetInvestmentByProjectIdForAuthor(projectId);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET FOR WALLET
        //public async Task<List<GetInvestmentDTO>> GetInvestmentForWallet(string walletType, ThisUserObj currentUser)
        //{
        //    try
        //    {
        //        if (await _investorRepository.GetInvestorById(Guid.Parse(currentUser.investorId)) == null)
        //        {
        //            throw new NotFoundException("No Investor Object Found!!!");
        //        }

        //        if (!walletType.Equals("I3") && !walletType.Equals("I4"))
        //            throw new InvalidFieldException("walletType must be I3 or I4!!!");

        //        List<Investment> investmentList = await _investmentRepository.GetInvestmentForWallet(Guid.Parse(currentUser.investorId), walletType.Equals("I3") ? ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString() : ProjectStatusEnum.ACTIVE.ToString());
        //        List<GetInvestmentDTO> list = _mapper.Map<List<GetInvestmentDTO>>(investmentList);

        //        foreach (GetInvestmentDTO item in list)
        //        {
        //            item.createDate = item.createDate == null ? null : await _validationService.FormatDateOutput(item.createDate);
        //            item.updateDate = item.updateDate == null ? null : await _validationService.FormatDateOutput(item.updateDate);
        //        }

        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //UPDATE
        //public async Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId)
        //{
        //    int result;
        //    try
        //    {
        //        if (investmentDTO.investorId == null || !await _validationService.CheckUUIDFormat(investmentDTO.investorId))
        //            throw new InvalidFieldException("Invalid investorId!!!");

        //        if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investmentDTO.investorId)))
        //            throw new NotFoundException("This investorId is not existed!!!");

        //        if (investmentDTO.projectId == null || !await _validationService.CheckUUIDFormat(investmentDTO.projectId))
        //            throw new InvalidFieldException("Invalid projectId!!!");

        //        if (!await _validationService.CheckExistenceId("Project", Guid.Parse(investmentDTO.projectId)))
        //            throw new NotFoundException("This projectId is not existed!!!");

        //        if (investmentDTO.packageId == null || !await _validationService.CheckUUIDFormat(investmentDTO.packageId))
        //            throw new InvalidFieldException("Invalid packageId!!!");

        //        if (!await _validationService.CheckExistenceId("Package", Guid.Parse(investmentDTO.packageId)))
        //            throw new NotFoundException("This packageId is not existed!!!");

        //        if (investmentDTO.quantity <= 0)
        //            throw new InvalidFieldException("quantity must be greater than 0!!!");

        //        if (investmentDTO.createBy != null && investmentDTO.createBy.Length >= 0)
        //        {
        //            if (investmentDTO.createBy.Equals("string"))
        //                investmentDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investmentDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investmentDTO.updateBy != null && investmentDTO.updateBy.Length >= 0)
        //        {
        //            if (investmentDTO.updateBy.Equals("string"))
        //                investmentDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investmentDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        Investment dto = _mapper.Map<Investment>(investmentDTO);
        //        result = await _investmentRepository.UpdateInvestment(dto, investmentId);
        //        if (result == 0)
        //            throw new UpdateObjectException("Can not update Investment Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}
