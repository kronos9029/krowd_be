using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;
using System.Data;

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
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _cache;


        public WithdrawRequestService(IWithdrawRequestRepository withdrawRequestRepository, 
            IValidationService validationService,
            IMapper mapper,
            IInvestorWalletRepository investorWalletRepository,
            IWalletTransactionService walletTransactionService,
            IAccountTransactionService accountTransactionService,
            IProjectWalletRepository projectWalletRepository,
            IInvestorRepository investorRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IDistributedCache cache)
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
            _userRepository = userRepository;
            _cache = cache;
        }


        public async Task<GetWithdrawRequestDTO> CreateWithdrawRequest(WithdrawRequestDTO request, ThisUserObj currentUser)
        {
            try
            {
                string newRequestId;
                string walletName = "";
                WithdrawRequest withdrawRequest = new();
                GetWithdrawRequestDTO getWithdrawRequestDTO = new();

                if (currentUser.roleId.Equals(currentUser.investorRoleId))
                    walletName = await _investorWalletRepository.GetInvertorWalletNamebyWalletId(withdrawRequest.FromWalletId);
                else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
                    walletName = await _projectWalletRepository.GetProjectWalletNameById(withdrawRequest.FromWalletId);

                List<Guid> admins = await _userRepository.GetUsersIdByRoleIdAndBusinessId(Guid.Parse(currentUser.adminRoleId), "");

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
                        CreateDate = DateTimePicker.GetDateTimeByTimeZone(),
                        FromWalletId = fromWallet.Id
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
                        CreateBy = withdrawRequest.CreateBy.ToString(),
                        FromWalletId = withdrawRequest.FromWalletId.ToString(),
                        FromWalletName = walletName
                    };
                    _walletTransactionService.TransferMoney(fromWallet, toWallet, request.Amount, currentUser.userId);

                    NotificationDetailDTO notification = new()
                    {
                        Title = "Investor "+currentUser.fullName+" vừa tạo yêu cầu rút tiền.",
                        EntityId = newRequestId
                    };
                    foreach (var admin in admins)
                    {
                        await DistributedCacheExtensions.UpdateNotification(_cache, admin.ToString(), notification);
                    }
                    
                    

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
                        CreateDate = DateTimePicker.GetDateTimeByTimeZone(),
                        FromWalletId = fromWallet.Id
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
                        CreateBy = withdrawRequest.CreateBy.ToString(),
                        FromWalletId = withdrawRequest.FromWalletId.ToString(),
                        FromWalletName = walletName
                };
                    _walletTransactionService.TransferMoney(fromWallet, toWallet, request.Amount, currentUser.userId);

                    NotificationDetailDTO notification = new()
                    {
                        Title = "Project Owner " + currentUser.fullName + " vừa tạo yêu cầu rút tiền.",
                        EntityId = newRequestId
                    };
                    
                    foreach (var admin in admins)
                    {
                        await DistributedCacheExtensions.UpdateNotification(_cache, admin.ToString(), notification);
                    }
                }
                return getWithdrawRequestDTO;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string receipt)
        {
            try
            {
                var resultString = "";

                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(request.Id), receipt);
                WithdrawRequest withdrawRequest = await _withdrawRequestRepository.GetWithdrawRequestByRequestId(Guid.Parse(request.Id));
                NotificationDetailDTO notification = new()
                {
                    Title = "Yêu cầu rút tiền của bạn đã được duyệt, xin hãy kiểm tra tài khoản và xác nhận.",
                    EntityId = withdrawRequest.Id.ToString()
                };
                string roleName = await _roleRepository.GetRoleNameByUserId((Guid)withdrawRequest.CreateBy);
                if (roleName == null || roleName.Equals(""))
                    throw new NotFoundException("User not Exits!!");
                else if (roleName.Equals(RoleEnum.INVESTOR.ToString()))
                {
                    Investor investor = await _investorRepository.GetInvestorByUserId((Guid)withdrawRequest.CreateBy);
                    InvestorWallet wallet = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, "I1");
                    resultString = await _accountTransactionService.CreateWithdrawAccountTransaction(wallet, withdrawRequest, currentUser.userId, roleName);

                    await DistributedCacheExtensions.UpdateNotification(_cache, withdrawRequest.CreateBy.ToString(), notification);
                }
                else if (roleName.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                {
                    ProjectWallet wallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType((Guid)withdrawRequest.CreateBy, "P1", null);
                    resultString = await _accountTransactionService.CreateWithdrawAccountTransaction(wallet, withdrawRequest, currentUser.userId, roleName);

                    await DistributedCacheExtensions.UpdateNotification(_cache, withdrawRequest.CreateBy.ToString(), notification);
                }
                    
                return resultString;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string receipt)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.AdminApproveWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(request.Id), receipt);
                NotificationDetailDTO notification = new()
                {
                    Title = "Bạn có phản hồi của Admin từ yêu cầu rút tiền.",
                    EntityId = request.Id
                };
                await DistributedCacheExtensions.UpdateNotification(_cache, request.CreateBy.ToString(), notification);
                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> ApproveWithdrawRequest(string userId, GetWithdrawRequestDTO request)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.UserApproveWithdrawRequest(Guid.Parse(userId), Guid.Parse(request.Id));

                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> AdminRejectWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO currentRequest, string RefusalReason)
        {
            try
            {
                Role requestCreatorRole = await _roleRepository.GetRoleByUserId(Guid.Parse(currentRequest.CreateBy));

                if (requestCreatorRole.Id.Equals(Guid.Parse(currentUser.investorRoleId)))
                {
                    InvestorWallet fromWallet = await _investorWalletRepository.GetInvestorWalletByUserIdAndWalletTypeId(Guid.Parse(currentRequest.CreateBy), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I1")));

                    InvestorWallet refundWallet = await _investorWalletRepository.GetInvestorWalletById(Guid.Parse(currentRequest.FromWalletId));

                    _walletTransactionService.TransferMoney(fromWallet, refundWallet, currentRequest.Amount, currentUser.userId);
                } else if (requestCreatorRole.Id.Equals(Guid.Parse(currentUser.projectManagerRoleId)))
                {
                    ProjectWallet refundWallet = await _projectWalletRepository.GetProjectWalletById(Guid.Parse(currentRequest.FromWalletId));

                    ProjectWallet fromWallet = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType(Guid.Parse(currentRequest.CreateBy), WalletTypeEnum.P1.ToString(), refundWallet.ProjectId);

                    _walletTransactionService.TransferMoney(fromWallet, refundWallet, currentRequest.Amount, currentUser.userId);
                }

                dynamic result = await _withdrawRequestRepository.AdminRejectWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(currentRequest.Id), RefusalReason);
                NotificationDetailDTO notification = new()
                {
                    Title = "Yêu cầu rút tiền của bạn đã bị từ chối, vui lòng liên hệ Krowd Help Center để biết thêm chi tiết.",
                    EntityId = currentRequest.Id
                };
                await DistributedCacheExtensions.UpdateNotification(_cache, currentRequest.CreateBy.ToString(), notification);

                return result;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> ReportWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string reportMessage)
        {
            try
            {
                dynamic result = await _withdrawRequestRepository.ReportWithdrawRequest(Guid.Parse(currentUser.userId), Guid.Parse(request.Id), reportMessage);
                NotificationDetailDTO notification = new()
                {
                    Title = "Bạn có phản hồi của "+currentUser.fullName+" từ yêu cầu rút tiền.",
                    EntityId = request.Id
                };
                await DistributedCacheExtensions.UpdateNotification(_cache, request.UpdateBy.ToString(), notification);
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
                string walletName = "";
                List<WithdrawRequest> withdrawRequestList = await _withdrawRequestRepository.GetWithdrawRequestByUserId(Guid.Parse(userId));
                //List<GetWithdrawRequestDTO> withdrawRequestDTOList = _mapper.Map<List<GetWithdrawRequestDTO>>(withdrawRequestList);
                List<GetWithdrawRequestDTO> withdrawRequestDTOList = new();

                Role role = await _roleRepository.GetRoleByUserId(Guid.Parse(userId));
                foreach (WithdrawRequest withdrawRequest in withdrawRequestList)
                {
                    if (role.Name.Equals(RoleEnum.INVESTOR.ToString()))
                        walletName = await _investorWalletRepository.GetInvertorWalletNamebyWalletId(withdrawRequest.FromWalletId);
                    if(role.Name.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                        walletName = await _projectWalletRepository.GetProjectWalletNameById(withdrawRequest.FromWalletId);
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
                        CreateDate = withdrawRequest.CreateDate.ToString(),
                        CreateBy = withdrawRequest.CreateBy.ToString(),
                        UpdateDate = withdrawRequest.UpdateBy.ToString(),
                        UpdateBy = withdrawRequest.UpdateBy.ToString(),
                        FromWalletId = withdrawRequest.FromWalletId.ToString(),
                        FromWalletName = walletName
                    };
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
                AllWithdrawRequestDTO result = new()
                {
                    listOfWithdrawRequest = new List<GetWithdrawRequestDTO>()
                };

                if (!Enum.IsDefined(typeof(WithdrawRequestEnum), filter)) throw new InvalidFieldException("filter must be ALL or PENDING or PARTIAL or PARTIAL_ADMIN or REJECTED or APPROVED!!!");

                List<WithdrawRequest> withdrawRequests = await _withdrawRequestRepository.GetAllWithdrawRequest(pageIndex, pageSize, userId, filter);
                result.numOfWithdrawRequest = await _withdrawRequestRepository.CountAllWithdrawRequest(userId, filter);
                Role role = await _roleRepository.GetRoleByUserId(Guid.Parse(userId));
                string walletName = "";
                foreach (WithdrawRequest withdrawRequest in withdrawRequests)
                {
                    if (role.Name.Equals(RoleEnum.INVESTOR.ToString()))
                        walletName = await _investorWalletRepository.GetInvertorWalletNamebyWalletId(withdrawRequest.FromWalletId);
                    if (role.Name.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                        walletName = await _projectWalletRepository.GetProjectWalletNameById(withdrawRequest.FromWalletId);
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
                        UpdateBy = withdrawRequest.UpdateBy.ToString(),
                        FromWalletId = withdrawRequest.FromWalletId.ToString(),
                        FromWalletName = walletName
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
                if (withdrawRequestDTO == null)
                    throw new NotFoundException("Withdraw Request Not Found!!");
                withdrawRequestDTO.CreateDate = await _validationService.FormatDateOutput(withdrawRequestDTO.CreateDate);
                withdrawRequestDTO.UpdateDate = await _validationService.FormatDateOutput(withdrawRequestDTO.UpdateDate);

                string walletName = "";
                Role role = await _roleRepository.GetRoleByUserId((Guid)withdrawRequest.CreateBy);
                if (role.Name.Equals(RoleEnum.INVESTOR.ToString()))
                    walletName = await _investorWalletRepository.GetInvertorWalletNamebyWalletId(withdrawRequest.FromWalletId);
                if (role.Name.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                    walletName = await _projectWalletRepository.GetProjectWalletNameById(withdrawRequest.FromWalletId);

                withdrawRequestDTO.FromWalletName = walletName;

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
