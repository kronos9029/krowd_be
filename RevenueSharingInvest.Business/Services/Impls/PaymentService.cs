using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IStageRepository _stageRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PaymentService(
            IPaymentRepository paymentRepository, 
            IInvestmentRepository investmentRepository, 
            IPeriodRevenueRepository periodRevenueRepository,
            IProjectRepository projectRepository,
            IPackageRepository packageRepository,
            IWalletTypeRepository walletTypeRepository,
            IStageRepository stageRepository, 
            IValidationService validationService, 
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _investmentRepository = investmentRepository;
            _periodRevenueRepository = periodRevenueRepository;
            _projectRepository = projectRepository;
            _packageRepository = packageRepository;
            _stageRepository = stageRepository;
            _walletTypeRepository = walletTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //GET ALL
        public async Task<dynamic> GetAllPayments(int pageIndex, int pageSize, string type, ThisUserObj currentUser)
        {
            dynamic result;
            try
            {
                if (!type.Equals(PaymentTypeEnum.INVESTMENT.ToString()) && !type.Equals(PaymentTypeEnum.PERIOD_REVENUE.ToString()))
                    throw new InvalidFieldException("Invalid type!!!");

                List<Payment> paymentList = await _paymentRepository.GetAllPayments(pageIndex, pageSize, type, Guid.Parse(currentUser.roleId), Guid.Parse(currentUser.userId));
                Project project = new Project();
                

                if (type.Equals(PaymentTypeEnum.INVESTMENT.ToString()))
                {
                    List<InvestmentPaymentDTO> list = _mapper.Map<List<InvestmentPaymentDTO>>(paymentList);
                    Package package = new Package();
                    foreach (InvestmentPaymentDTO item in list)
                    {                  
                        package = await _packageRepository.GetPackageById(Guid.Parse(item.packageId));
                        project = await _projectRepository.GetProjectById(package.ProjectId);
                        item.createDate = await _validationService.FormatDateOutput(item.createDate);
                        item.projectId = project.Id.ToString();
                        item.projectName = project.Name;
                        item.packageId = package.Id.ToString();
                        item.packageName = package.Name;                   
                        item.investedQuantity = (int)(item.amount / package.Price);
                        item.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))).Name;
                    }
                    result = list;
                }
                else
                {
                    List<PeriodRevenuePaymentDTO> list = _mapper.Map<List<PeriodRevenuePaymentDTO>>(paymentList);
                    PeriodRevenue periodRevenue = new PeriodRevenue();
                    Stage stage = new Stage();
                    foreach (PeriodRevenuePaymentDTO item in list)
                    {
                        stage = await _stageRepository.GetStageById(Guid.Parse(item.stageId));
                        project = await _projectRepository.GetProjectById(stage.ProjectId);                       
                        item.createDate = await _validationService.FormatDateOutput(item.createDate);
                        item.projectId = project.Id.ToString();
                        item.projectName = project.Name;
                        item.stageId = stage.Id.ToString();
                        item.stageName = stage.Name;
                        item.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2")))).Name;
                    }
                    result = list;
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
        public async Task<dynamic> GetPaymentById(Guid paymentId, ThisUserObj currentUser)
        {
            dynamic result;
            try
            {
                Payment payment = await _paymentRepository.GetPaymentById(paymentId);
                if (payment == null)
                    throw new NotFoundException("No Payment Object Found!!!");
                if (!currentUser.userId.Equals(payment.FromId.ToString()) && !currentUser.userId.Equals(payment.ToId.ToString()))
                    throw new InvalidFieldException("This id is not belong to your payments!!!");

                if (payment.Type.Equals(PaymentTypeEnum.INVESTMENT.ToString()))
                {
                    result = _mapper.Map<InvestmentPaymentDTO>(payment);
                    Package package = await _packageRepository.GetPackageById(Guid.Parse(result.packageId));
                    Project project = await _projectRepository.GetProjectById(package.ProjectId);
                    result.createDate = await _validationService.FormatDateOutput(result.createDate);
                    result.projectId = project.Id.ToString();
                    result.projectName = project.Name;
                    result.packageId = package.Id.ToString();
                    result.packageName = package.Name;
                    result.investedQuantity = (int)(result.amount / package.Price);
                    result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))).Name;
                }
                else
                {
                    result = _mapper.Map<PeriodRevenuePaymentDTO>(payment);
                    Stage stage = await _stageRepository.GetStageById(Guid.Parse(result.stageId));
                    Project project = await _projectRepository.GetProjectById(stage.ProjectId);
                    result.createDate = await _validationService.FormatDateOutput(result.createDate);
                    result.projectId = project.Id.ToString();
                    result.projectName = project.Name;
                    result.stageId = stage.Id.ToString();
                    result.stageName = stage.Name;
                    result.fromWalletName = (await _walletTypeRepository.GetWalletTypeById(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2")))).Name;
                }
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
