using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    public class MoneyOverviewService : IMoneyOverviewService
    {
        private readonly IAccountTransactionRepository _accountTransactionRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public MoneyOverviewService(
            IAccountTransactionRepository accountTransactionRepository, 
            IInvestmentRepository investmentRepository,
            IPaymentRepository paymentRepository,
            IProjectRepository projectRepository, 
            IValidationService validationService, 
            IMapper mapper)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _investmentRepository = investmentRepository;
            _paymentRepository = paymentRepository;
            _projectRepository = projectRepository;

            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<MoneyOverviewDTO> GetMoneyOverviewForInvestor(ThisUserObj currentUser)
        {
            MoneyOverviewDTO result = new MoneyOverviewDTO();
            try
            {
                List<AccountTransaction> accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions(0, 0, currentUser.userId, "");
                List<Payment> investmentPaymentList = await _paymentRepository.GetAllPayments(0, 0, PaymentTypeEnum.INVESTMENT.ToString(), Guid.Parse(currentUser.roleId), Guid.Parse(currentUser.userId));
                List<Payment> periodRevenuePaymentList = await _paymentRepository.GetAllPayments(0, 0, PaymentTypeEnum.PERIOD_REVENUE.ToString(), Guid.Parse(currentUser.roleId), Guid.Parse(currentUser.userId));
                List<Project> projectList = await _projectRepository.GetInvestedProjects(0, 0, Guid.Parse(currentUser.investorId));
                List<Investment> investmentList = await _investmentRepository.GetAllInvestments(0, 0, null, null, null, currentUser.investorId, null, Guid.Parse(currentUser.roleId));
                

                result.totalDepositedAmount = (double)accountTransactionList
                    .Where(accountTransaction => accountTransaction.Type.Equals("TOP-UP") && accountTransaction.ResultCode == 0)
                    .Sum(accountTransaction => accountTransaction.Amount);

                result.totalWithdrawedAmount = (double)accountTransactionList
                    .Where(accountTransaction => accountTransaction.Type.Equals("WITHDRAW") && accountTransaction.ResultCode == 0)
                    .Sum(accountTransaction => accountTransaction.Amount); ;

                result.totalInvestedAmount = (double)investmentPaymentList
                    .Where(investmentPayment => investmentPayment.Status.Equals(TransactionStatusEnum.SUCCESS.ToString()))
                    .Sum(investmentPayment => investmentPayment.Amount);

                result.totalReceivedAmount = (double)periodRevenuePaymentList
                    .Where(periodRevenuePayment => periodRevenuePayment.Status.Equals(TransactionStatusEnum.SUCCESS.ToString()))
                    .Sum(periodRevenuePayment => periodRevenuePayment.Amount);

                result.numOfInvestedProject = projectList.Count();

                result.numOfCallingForInvestmentInvestedProject = projectList
                    .FindAll(project => project.Status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString())).Count;

                result.numOfCallingTimeIsOverInvestedProject = projectList
                    .FindAll(project => project.Status.Equals(ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString())).Count;

                result.numOfActiveInvestedProject = projectList
                    .FindAll(project => project.Status.Equals(ProjectStatusEnum.ACTIVE.ToString())).Count;

                result.numOfInvestment = investmentList.Count;

                result.numOfSuccessInvestment = investmentList
                    .FindAll(investment => investment.Status.Equals(TransactionStatusEnum.SUCCESS.ToString())).Count;

                result.numOfFailedInvestment = investmentList
                    .FindAll(investment => investment.Status.Equals(TransactionStatusEnum.FAILED.ToString())).Count;

                result.numOfCanceledInvestment = investmentList
                    .FindAll(investment => investment.Status.Equals(TransactionStatusEnum.CANCELED.ToString())).Count;

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
