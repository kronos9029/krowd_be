using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class WithdrawRequestService : IWithdrawRequestService
    {
        private readonly IWithdrawRequestRepository _withdrawRequestRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IAccountTransactionService _accountTransactionService;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IRoleRepository _roleRepository;


        public WithdrawRequestService(IWithdrawRequestRepository withdrawRequestRepository, 
            IValidationService validationService, 
            IMapper mapper, 
            IInvestorWalletRepository investorWalletRepository, 
            IWalletTransactionService walletTransactionService,
            IAccountTransactionService accountTransactionService,
            IProjectWalletRepository projectWalletRepository,
            IInvestorRepository investorRepository,
            IRoleRepository roleRepository)
        {
            _withdrawRequestRepository = withdrawRequestRepository;
            _validationService = validationService;
            _mapper = mapper;
            _investorWalletRepository = investorWalletRepository;
            _walletTransactionService = walletTransactionService;
            _accountTransactionService = accountTransactionService;
            _projectWalletRepository = projectWalletRepository;
            _roleRepository = roleRepository;
            _investorRepository = investorRepository;
        }


        public async Task<GetWithdrawRequestDTO> CreateInvestorWithdrawRequest(WithdrawRequestDTO request, ThisUserObj currentUser)
        {
            try
            {
                string newRequestId;
                WithdrawRequest withdrawRequest = new();
                GetWithdrawRequestDTO getWithdrawRequestDTO = new();                

                if (currentUser.roleId.Equals(currentUser.investorRoleId)){

                    InvestorWallet fromWallet = await _investorWalletRepository.GetInvestorWalletById(Guid.Parse(request.FromWalletId));

                    if (!fromWallet.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))
                        && !fromWallet.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5"))))
                        throw new InvalidFieldException("FromWalletId type must be I2 or I5!!!");

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
                        Description = "Withdraw Money",
                        CreateDate = DateTimePicker.GetDateTimeByTimeZone()
                    };

                    newRequestId = await _withdrawRequestRepository.CreateWithdrawRequest(withdrawRequest);
                    if (newRequestId == null || newRequestId.Equals(""))
                        throw new CreateObjectException("Withdraw Request Failed!!");

                    withdrawRequest.Id = Guid.Parse(newRequestId);

                    getWithdrawRequestDTO = new()
                    {
                        Id = newRequestId,
                        BankName = withdrawRequest.BankName,
                        AccountName = withdrawRequest.AccountName,
                        BankAccount = withdrawRequest.BankAccount,
                        Description = withdrawRequest.Description,
                        Amount = withdrawRequest.Amount,
                        Status = withdrawRequest.Status,
                        RefusalReason = withdrawRequest.RefusalReason,
                        CreateDate = withdrawRequest.CreateDate.ToString(),
                        CreateBy = withdrawRequest.CreateBy.ToString()
                    };
                    _walletTransactionService.TransferMoney(fromWallet, toWallet, request.Amount, currentUser.userId);

                } else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
                {
                    ProjectWallet fromWallet = await _projectWalletRepository.GetProjectWalletById(Guid.Parse(request.FromWalletId));

                    if (!fromWallet.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2"))) 
                        && !fromWallet.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5"))))
                        throw new InvalidFieldException("FromWalletId type must be P2 or P5!!!");

                    if (request.Amount < 0 || request.Amount > fromWallet.Balance)
                        throw new WalletBalanceException("You Don't Have Enough Money To Withdraw!!");

                    ProjectWallet toWallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(Guid.Parse(currentUser.userId), "P1", null);
                    
                    withdrawRequest = new()
                    {
                        BankName = request.BankName,
                        BankAccount = request.BankAccount,
                        AccountName = request.AccountName,
                        Amount = request.Amount,
                        CreateBy = Guid.Parse(currentUser.userId),
                        Status = WithdrawRequestEnum.PENDING.ToString(),
                        Description = "Withdraw Money",
                        CreateDate = DateTimePicker.GetDateTimeByTimeZone()
                    };

                    newRequestId = await _withdrawRequestRepository.CreateWithdrawRequest(withdrawRequest);
                    if (newRequestId == null || newRequestId.Equals(""))
                        throw new CreateObjectException("Withdraw Request Failed!!");

                    withdrawRequest.Id = Guid.Parse(newRequestId);

                    getWithdrawRequestDTO = new()
                    {
                        Id = newRequestId,
                        BankName = withdrawRequest.BankName,
                        AccountName = withdrawRequest.AccountName,
                        BankAccount = withdrawRequest.BankAccount,
                        Description = withdrawRequest.Description,
                        Amount = withdrawRequest.Amount,
                        Status = withdrawRequest.Status,
                        RefusalReason = withdrawRequest.RefusalReason,
                        CreateDate = withdrawRequest.CreateDate.ToString(),
                        CreateBy = withdrawRequest.CreateBy.ToString()
                    };
                    _walletTransactionService.TransferMoney(fromWallet, toWallet, request.Amount, currentUser.userId);
                }
                return getWithdrawRequestDTO;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, string requestId, string receipt)
        {
            try
            {
                var resultString = "";
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(requestId), receipt);
                WithdrawRequest withdrawRequest = await _withdrawRequestRepository.GetWithdrawRequestByRequestId(Guid.Parse(requestId));

                string roleName = await _roleRepository.GetRoleNameByUserId((Guid)withdrawRequest.CreateBy);
                if (roleName == null || roleName.Equals(""))
                    throw new NotFoundException("User not Exits!!");
                else if (roleName.Equals(RoleEnum.INVESTOR.ToString()))
                {
                    Investor investor = await _investorRepository.GetInvestorByUserId((Guid)withdrawRequest.CreateBy);
                    InvestorWallet wallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, "I1");
                    resultString = await _accountTransactionService.CreateWithdrawAccountTransaction(wallet, withdrawRequest, currentUser.userId, roleName);
                }
                else if (roleName.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                {
                    ProjectWallet wallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType((Guid)withdrawRequest.CreateBy, "P1", null);
                    resultString = await _accountTransactionService.CreateWithdrawAccountTransaction(wallet, withdrawRequest, currentUser.userId, roleName);
                }
                    
                return resultString;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, string requestId, string receipt)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(requestId), receipt);
                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> ApproveWithdrawRequest(string userId, string requestId)
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

        public async Task<dynamic> ReportWithdrawRequest(string userId, string requestId, string reportMessage)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.ReportWithdrawRequest(Guid.Parse(userId), Guid.Parse(requestId), reportMessage);
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

        //GET ALL
        public async Task<AllWithdrawRequestDTO> GetAllWithdrawRequest(int pageIndex, int pageSize, string userId, string filter)
        {
            try
            {
                AllWithdrawRequestDTO result = new AllWithdrawRequestDTO();
                result.listOfWithdrawRequest = new List<GetWithdrawRequestDTO>();

                if (!Enum.IsDefined(typeof(WithdrawRequestEnum), filter)) throw new InvalidFieldException("filter must be ALL or PENDING or PARTIAL or PARTIAL_ADMIN or REJECTED or APPROVED!!!");

                List<WithdrawRequest> withdrawRequests = await _withdrawRequestRepository.GetAllWithdrawRequest(pageIndex, pageSize, userId, filter);
                result.numOfWithdrawRequest = await _withdrawRequestRepository.CountAllWithdrawRequest(userId, filter);

                foreach (WithdrawRequest withdrawRequest in withdrawRequests)
                {
                    GetWithdrawRequestDTO withdrawRequestDTO = new()
                    {
                        Id = withdrawRequest.Id.ToString(),
                        BankName = withdrawRequest.BankName,
                        AccountName = withdrawRequest.AccountName,
                        BankAccount = withdrawRequest.BankAccount,
                        Description = withdrawRequest.Description,
                        Amount = withdrawRequest.Amount,
                        Status = withdrawRequest.Status,
                        RefusalReason = withdrawRequest.RefusalReason,
                        CreateDate = await _validationService.FormatDateOutput(withdrawRequest.CreateDate.ToString()),
                        CreateBy = withdrawRequest.CreateBy.ToString(),
                        UpdateDate = await _validationService.FormatDateOutput(withdrawRequest.UpdateDate.ToString()),
                        UpdateBy = withdrawRequest.UpdateBy.ToString()
                    };
                    result.listOfWithdrawRequest.Add(withdrawRequestDTO);
                }
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id)
        {
            try
            {
                WithdrawRequest withdrawRequest = await _withdrawRequestRepository.GetWithdrawRequestByRequestId(Guid.Parse(id));
                GetWithdrawRequestDTO withdrawRequestDTO = _mapper.Map<GetWithdrawRequestDTO>(withdrawRequest);
                withdrawRequestDTO.CreateDate = await _validationService.FormatDateOutput(withdrawRequestDTO.CreateDate);
                withdrawRequestDTO.UpdateDate = await _validationService.FormatDateOutput(withdrawRequestDTO.UpdateDate);
                return withdrawRequestDTO;
            }
            
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
