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
            IMapper mapper)
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
        }

        //CREATE
        public async Task<InvestmentPaymentDTO> CreateInvestment(CreateInvestmentDTO investmentDTO, ThisUserObj currentUser)
        {
            InvestmentPaymentDTO result = new InvestmentPaymentDTO();
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
                    payment.Amount = investment.TotalPrice;
                    payment.Description = "Đầu tư gói '" + package.Name + "' x" + investmentDTO.quantity;
                    payment.Type = TransactionTypeEnum.INVESTMENT.ToString();
                    payment.FromId = Guid.Parse(currentUser.userId);
                    payment.ToId = projectManager.Id;
                    payment.CreateBy = Guid.Parse(currentUser.userId);
                    payment.Status = TransactionStatusEnum.FAILED.ToString();

                    string paymentId = await _paymentRepository.CreatePayment(payment);

                    result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                    result.createDate = await _validationService.FormatDateOutput(result.createDate);
                    Project project = await _projectRepository.GetProjectById(package.ProjectId);
                    result.projectId = project.Id.ToString();
                    result.projectName = project.Name;
                    result.packageId = package.Id.ToString();
                    result.packageName = package.Name;
                    result.investedQuantity = investmentDTO.quantity;
                    result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")))).Name;
                    result.fee = "0%";
                }
                else
                {
                    //Create Payment
                    Payment payment = new Payment();
                    User projectManager = await _userRepository.GetProjectManagerByProjectId(package.ProjectId);
                    payment.InvestmentId = Guid.Parse(investmentId);
                    payment.Amount = investment.TotalPrice;
                    payment.Description = "Đầu tư gói '" + package.Name + "' x" + investmentDTO.quantity;
                    payment.Type = TransactionTypeEnum.INVESTMENT.ToString();
                    payment.FromId = Guid.Parse(currentUser.userId);
                    payment.ToId = projectManager.Id;
                    payment.CreateBy = Guid.Parse(currentUser.userId);
                    payment.Status = TransactionStatusEnum.SUCCESS.ToString();

                    string paymentId = await _paymentRepository.CreatePayment(payment);
                    if (paymentId != "")
                    {
                        //Update Investment Status
                        investment = new Investment();
                        investment.Id = Guid.Parse(investmentId);
                        investment.UpdateBy = Guid.Parse(currentUser.userId);
                        investment.Status = TransactionStatusEnum.SUCCESS.ToString();
                        await _investmentRepository.UpdateInvestmentStatus(investment);

                        //Update Package Quantity
                        await _packageRepository.UpdatePackageRemainingQuantity(package.Id, package.RemainingQuantity - investmentDTO.quantity, Guid.Parse(currentUser.userId));

                        //Update Project Amount
                        await _projectRepository.UpdateProjectInvestedCapitalAndRemainAmount(package.ProjectId, (double)payment.Amount, Guid.Parse(currentUser.userId));

                        //Subtract I2 balance
                        InvestorWallet investorWallet = new InvestorWallet();
                        investorWallet.InvestorId = Guid.Parse(currentUser.investorId);
                        investorWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2"));
                        investorWallet.Balance = -payment.Amount;
                        investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        //Create WalletTransaction from I2 to I3
                        WalletTransaction walletTransaction = new WalletTransaction();
                        walletTransaction.PaymentId = Guid.Parse(paymentId);
                        walletTransaction.Amount = payment.Amount;
                        walletTransaction.Fee = 0;
                        walletTransaction.Description = "Transfer from I2 to I3 to invest";
                        walletTransaction.FromWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString())).Id;
                        walletTransaction.ToWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString())).Id;
                        walletTransaction.Type = TransactionTypeEnum.INVESTMENT.ToString();
                        walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        //Add I3 balance
                        investorWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"));
                        investorWallet.Balance = payment.Amount;
                        await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        ////Subtract I3 balance
                        //investorWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"));
                        //investorWallet.Balance = -payment.Amount;
                        //await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                        ////Create WalletTransaction from I3 to P3
                        //walletTransaction.PaymentId = Guid.Parse(paymentId);
                        //walletTransaction.InvestorWalletId = (await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I3.ToString())).Id;
                        //walletTransaction.ProjectWalletId = (await _projectWalletRepository.GetProjectWalletByProjectOwnerIdAndType(projectManager.Id, WalletTypeEnum.B3.ToString())).Id;
                        //walletTransaction.Amount = payment.Amount;
                        //walletTransaction.Fee = 0;
                        //walletTransaction.Description = "Transfer from I3 to P3 to invest";
                        //walletTransaction.FromWalletId = walletTransaction.InvestorWalletId;
                        //walletTransaction.ToWalletId = walletTransaction.ProjectWalletId;
                        //walletTransaction.Type = TransactionTypeEnum.INVESTMENT.ToString();
                        //walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                        //await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                        ////Add P3 balance
                        //ProjectWallet projectWallet = new ProjectWallet();
                        //projectWallet.ProjectManagerId = projectManager.Id;
                        //projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3"));
                        //projectWallet.Balance = payment.Amount;
                        //projectWallet.UpdateBy = Guid.Parse(currentUser.userId);
                        //await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                        //Format Payment response
                        result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                        result.createDate = await _validationService.FormatDateOutput(result.createDate);
                        Project project = await _projectRepository.GetProjectById(package.ProjectId);
                        result.projectId = project.Id.ToString();
                        result.projectName = project.Name;
                        result.packageId = package.Id.ToString();
                        result.packageName = package.Name;
                        result.investedQuantity = investmentDTO.quantity;
                        result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")))).Name;
                        result.fee = walletTransaction.Fee + "%";
                    }
                    else
                    {
                        payment.Status = TransactionStatusEnum.FAILED.ToString();
                        paymentId = await _paymentRepository.CreatePayment(payment);
                        result = _mapper.Map<InvestmentPaymentDTO>(await _paymentRepository.GetPaymentById(Guid.Parse(paymentId)));
                        result.createDate = await _validationService.FormatDateOutput(result.createDate);
                        Project project = await _projectRepository.GetProjectById(package.ProjectId);
                        result.projectId = project.Id.ToString();
                        result.projectName = project.Name;
                        result.packageId = package.Id.ToString();
                        result.packageName = package.Name;
                        result.investedQuantity = investmentDTO.quantity;
                        result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")))).Name;
                        result.fee = "0%";

                        investment = new Investment();
                        investment.Id = Guid.Parse(investmentId);
                        investment.UpdateBy = Guid.Parse(currentUser.userId);
                        investment.Status = TransactionStatusEnum.FAILED.ToString();
                        await _investmentRepository.UpdateInvestmentStatus(investment);
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
        public async Task<List<GetInvestmentDTO>> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, ThisUserObj currentUser)
        {
            try
            {
                string projectStatus = null;

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

                    projectStatus = walletTypeId.Equals(
                        WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"), 
                        StringComparison.InvariantCultureIgnoreCase) ? ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString() : ProjectStatusEnum.ACTIVE.ToString();
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

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId, StringComparison.InvariantCultureIgnoreCase) 
                    || currentUser.roleId.Equals(currentUser.projectManagerRoleId, StringComparison.InvariantCultureIgnoreCase))
                {
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

                List<Investment> investmentList = await _investmentRepository.GetAllInvestments(pageIndex, pageSize, projectStatus, businessId, projectId, investorId, Guid.Parse(currentUser.roleId));
                List<GetInvestmentDTO> list = _mapper.Map<List<GetInvestmentDTO>>(investmentList);

                foreach (GetInvestmentDTO item in list)
                {
                    item.createDate = item.createDate == null ? null : await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = item.updateDate == null ? null : await _validationService.FormatDateOutput(item.updateDate);
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
