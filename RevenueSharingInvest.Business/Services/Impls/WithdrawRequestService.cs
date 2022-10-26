using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class WithdrawRequestService : IWithdrawRequestService
    {
        private readonly IWithdrawRequestRepository _withdrawRequestRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IAccountTransactionService _accountTransactionService;


        public WithdrawRequestService(IWithdrawRequestRepository withdrawRequestRepository, 
            IValidationService validationService, 
            IMapper mapper, 
            IInvestorWalletRepository investorWalletRepository, 
            IWalletTransactionService walletTransactionService,
            IAccountTransactionService accountTransactionService)
        {
            _withdrawRequestRepository = withdrawRequestRepository;
            _validationService = validationService;
            _mapper = mapper;
            _investorWalletRepository = investorWalletRepository;
            _walletTransactionService = walletTransactionService;
            _accountTransactionService = accountTransactionService;
        }


        public async Task<WithdrawRequest> CreateInvestorWithdrawRequest(InvestorWithdrawRequest request, ThisUserObj currentUser)
        {
            try
            {
                string newRequestId;
                WithdrawRequest withdrawRequest = new();
                if (currentUser.roleId.Equals(currentUser.investorRoleId)){
                    InvestorWallet fromWallet = await _investorWalletRepository.GetInvestorWalletById(Guid.Parse(request.FromWalletId));

                    if (request.Amount < 0 || request.Amount > fromWallet.Balance)
                        throw new WalletBalanceException("You Don't Have Enough Money To Withdraw!!");

                    
                    InvestorWallet toWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), "I1");

                    withdrawRequest = new()
                    {
                        BankName = request.BankName,
                        BankAccount = request.BankAccount,
                        AccountName = request.AccountName,
                        Amount = request.Amount,
                        CreateBy = Guid.Parse(currentUser.userId),
                        Status = WithdrawRequestEnum.PENDING.ToString(),
                        Description = "Withdraw Money"
                    };

                    newRequestId = await _withdrawRequestRepository.CreateWithdrawRequest(withdrawRequest);
                    if (newRequestId == null || newRequestId.Equals(""))
                        throw new CreateObjectException("Withdraw Request Failed!!");
                    
                    _walletTransactionService.TransferMoney(fromWallet, toWallet, request.Amount, currentUser.userId);

                }
                return withdrawRequest;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, string requestId, double amount)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(requestId));

                InvestorWallet investorWallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), "I1");
                if (investorWallet == null)
                    throw new NotFoundException("No Such Wallet With That ID!!");

                var resultString = await _accountTransactionService.CreateWithdrawAccountTransaction(investorWallet, currentUser.userId, amount, requestId);
                return resultString;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, string requestId)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(requestId));
                return result;

            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> InvestorApproveWithdrawRequest(string userId, string requestId)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.InvestorApproveWithdrawRequest(Guid.Parse(userId), Guid.Parse(requestId));

                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminRejectWithdrawRequest(string userId, string requestId, string RefusalReason)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminRejectWithdrawRequest(Guid.Parse(userId), Guid.Parse(requestId), RefusalReason);

                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> InvestorReportWithdrawRequest(string userId, string requestId, string description)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.InvestorReportWithdrawRequest(Guid.Parse(userId), Guid.Parse(requestId), description);
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<GetWithdrawRequestDTO> GetWithdrawRequestByRequestIdAndUserId(string requestId, string userId)
        {
            try
            {
                WithdrawRequest withdrawRequest = await _withdrawRequestRepository.GetWithdrawRequestByRequestIdAndUserId(Guid.Parse(requestId), Guid.Parse(userId));
                GetWithdrawRequestDTO withdrawRequestDTO = _mapper.Map<GetWithdrawRequestDTO>(withdrawRequest);
                return withdrawRequestDTO;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<List<GetWithdrawRequestDTO>> GetWithdrawRequestByUserId(string userId)
        {
            try
            {
                List<WithdrawRequest> withdrawRequestList = await _withdrawRequestRepository.GetWithdrawRequestByUserId(Guid.Parse(userId));
                //List<GetWithdrawRequestDTO> withdrawRequestDTOList = _mapper.Map<List<GetWithdrawRequestDTO>>(withdrawRequestList);
                List<GetWithdrawRequestDTO> withdrawRequestDTOList = new();
                

                foreach (WithdrawRequest withdrawRequest in withdrawRequestList)
                {
                    GetWithdrawRequestDTO withdrawRequestDTO = new();
                    withdrawRequestDTO.Id = withdrawRequest.Id.ToString();
                    withdrawRequestDTO.BankName = withdrawRequest.BankName;
                    withdrawRequestDTO.AccountName = withdrawRequest.AccountName;
                    withdrawRequestDTO.BankAccount = withdrawRequest.BankAccount;
                    withdrawRequestDTO.Description = withdrawRequest.Description;
                    withdrawRequestDTO.Amount = withdrawRequest.Amount;
                    withdrawRequestDTO.Status = withdrawRequest.Status;
                    withdrawRequestDTO.RefusalReason = withdrawRequest.RefusalReason;
                    withdrawRequestDTO.CreateDate = withdrawRequest.CreateDate.ToString();
                    withdrawRequestDTO.CreateBy = withdrawRequest.CreateBy.ToString();
                    withdrawRequestDTO.UpdateDate = withdrawRequest.UpdateBy.ToString();
                    withdrawRequestDTO.UpdateBy = withdrawRequest.UpdateBy.ToString();
                    withdrawRequestDTOList.Add(withdrawRequestDTO);
                }

                return withdrawRequestDTOList;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }


    }
}
