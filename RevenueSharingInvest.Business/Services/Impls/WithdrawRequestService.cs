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


        public WithdrawRequestService(IWithdrawRequestRepository withdrawRequestRepository, IValidationService validationService, IMapper mapper, IInvestorWalletRepository investorWalletRepository)
        {
            _withdrawRequestRepository = withdrawRequestRepository;
            _validationService = validationService;
            _mapper = mapper;
            _investorWalletRepository = investorWalletRepository;
        }


        public async Task<string> CreateInvestorWithdrawRequest(InvestorWithdrawRequest request, ThisUserObj currentUser)
        {
            try
            {
                string newRequestId = "";
                if (currentUser.roleId.Equals(currentUser.investorRoleId)){
                    double remainBalance = await _investorWalletRepository.GetInvestorWalletBalanceById(Guid.Parse(request.WalletId));

                    if (request.Amount < 0 || request.Amount > remainBalance)
                        throw new WalletBalanceException("Invalid Amount!!");

                    WithdrawRequest withdrawRequest = _mapper.Map<WithdrawRequest>(request);

                    withdrawRequest.CreateBy = Guid.Parse(currentUser.userId);
                    withdrawRequest.Status = WithdrawRequestEnum.PENDING.ToString();

                    newRequestId = await _withdrawRequestRepository.CreateWithdrawRequest(withdrawRequest);
                }


                return newRequestId;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminApproveWithdrawRequest(string userId, string requestId)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(userId), Guid.Parse(requestId));
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
                List<GetWithdrawRequestDTO> withdrawRequestDTOList = _mapper.Map<List<GetWithdrawRequestDTO>>(withdrawRequestList);
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
