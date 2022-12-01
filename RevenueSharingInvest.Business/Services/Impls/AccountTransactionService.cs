using AutoMapper;
using Firebase.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models;
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
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class AccountTransactionService : IAccountTransactionService
    {
        private readonly IAccountTransactionRepository _accountTransactionRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;


        public AccountTransactionService(IAccountTransactionRepository accountTransactionRepository, 
            IValidationService validationService, 
            IMapper mapper, 
            IInvestorWalletRepository investorWalletRepository,
            IWalletTypeRepository walletTypeRepository,
            IInvestorRepository investorRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IRoleService roleService,
            IProjectWalletRepository projectWalletRepository,
            IDistributedCache cache,
            IUserRepository userRepository)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _investorWalletRepository = investorWalletRepository;
            _validationService = validationService;
            _walletTypeRepository = walletTypeRepository;
            _mapper = mapper;
            _investorRepository = investorRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _roleService = roleService;
            _projectWalletRepository = projectWalletRepository;
            _cache = cache;
            _userRepository = userRepository;
        }

        //CREATE
        public async Task<string> CreateTopUpAccountTransaction(MomoPaymentResult momoPaymentResult)
        {
            try
            {
                //if (accountTransactionDTO.fromUserId == null || !await _validationService.CheckUUIDFormat(accountTransactionDTO.fromUserId))
                //    throw new InvalidFieldException("Invalid fromUserId!!!");

                //if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(accountTransactionDTO.fromUserId)))
                //    throw new NotFoundException("This fromUserId is not existed!!!");

                //if (accountTransactionDTO.toUserId == null || !await _validationService.CheckUUIDFormat(accountTransactionDTO.toUserId))
                //    throw new InvalidFieldException("Invalid toUserId!!!");

                //if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(accountTransactionDTO.toUserId)))
                //    throw new NotFoundException("This toUserId is not existed!!!");

                //if (accountTransactionDTO.description != null && (accountTransactionDTO.description.Equals("string") || accountTransactionDTO.description.Length == 0))
                //    accountTransactionDTO.description = null;

                //if (!await _validationService.CheckText(accountTransactionDTO.status))
                //    throw new InvalidFieldException("Invalid status!!!");

                //if (accountTransactionDTO.createBy != null && accountTransactionDTO.createBy.Length >= 0)
                //{
                //    if (accountTransactionDTO.createBy.Equals("string"))
                //        accountTransactionDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(accountTransactionDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (accountTransactionDTO.updateBy != null && accountTransactionDTO.updateBy.Length >= 0)
                //{
                //    if (accountTransactionDTO.updateBy.Equals("string"))
                //        accountTransactionDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(accountTransactionDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}


                AccountTransaction accountTransaction = _mapper.Map<AccountTransaction>(momoPaymentResult);
                accountTransaction.PartnerClientId = Guid.Parse(momoPaymentResult.partnerClientId);
                accountTransaction.FromUserId = accountTransaction.PartnerClientId;
                accountTransaction.Type = "TOP-UP";

                NotificationDetailDTO notification = new();
                

                string id = await _accountTransactionRepository.CreateAccountTransaction(accountTransaction);
                if (id.Equals(""))
                    throw new CreateObjectException("Can not create AccountTransaction Object!");

                if(accountTransaction.ResultCode == 0)
                {
                    string roleName = await _roleService.GetRoleNameByUserId(momoPaymentResult.partnerClientId);
                    if (roleName.Equals(RoleEnum.INVESTOR.ToString()))
                    {
                        Investor investor = await _investorRepository.GetInvestorByUserId((Guid)accountTransaction.PartnerClientId);
                        InvestorTopUp(accountTransaction, investor);
                        string moneySeparator = await _validationService.FormatMoney(momoPaymentResult.amount.ToString());
                        notification.Title = "Bạn vừa nạp "+moneySeparator+"VNĐ vào ví đầu tư chung.";
                        await NotificationCache.UpdateNotification(_cache, momoPaymentResult.partnerClientId, notification);

                        DeviceToken deviceToken = await DeviceTokenCache.GetAvailableDevice(_cache, accountTransaction.PartnerClientId.ToString());
                        PushNotification pushNotification = new()
                        {
                            Title = "Tiền về ví Krowd!!",
                            Body = notification.Title
                        };
                        await FirebasePushNotification.SendMultiDevicePushNotification(deviceToken.Tokens, pushNotification);
                    }
                    if (roleName.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                    {
                        ProjectOwnerTopUp(accountTransaction);
                        notification.Title = "Bạn vừa nạp " + momoPaymentResult.amount + "VNĐ vào ví thanh toán chung.";
                        await NotificationCache.UpdateNotification(_cache, momoPaymentResult.partnerClientId, notification);
                    }
                        
                }

                return id;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async void InvestorTopUp(AccountTransaction accountTransaction, Investor investor)
        {
            try
            {
                //investor top-up I1 Wallet
                double realAmount = Convert.ToDouble(accountTransaction.Amount);
                InvestorWallet I1 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, WalletTypeEnum.I1.ToString());
                I1.Balance += realAmount;
                I1.UpdateDate = DateTimePicker.GetDateTimeByTimeZone();
                I1.UpdateBy = (Guid)accountTransaction.FromUserId;

                if ((await _investorWalletRepository.UpdateWalletBalance(I1)) == 0)
                    throw new CreateObjectException("Investor Top Up Failed!!");

                //Create CASH_IN WalletTransaction to I1
                WalletTransaction walletTransaction = new()
                {
                    Amount = realAmount,
                    Fee = 0,
                    Description = "Deposit money into I1 wallet",
                    ToWalletId = I1.Id,
                    Type = WalletTransactionTypeEnum.DEPOSIT.ToString(),
                    CreateBy = accountTransaction.FromUserId
                };
                if((await _walletTransactionRepository.CreateWalletTransaction(walletTransaction)).Equals(""))
                    throw new CreateObjectException("Investor Top Up Failed!!");


                //Money From I1 Wallet automaticly tranfer from I1 to I2
                InvestorWallet I2 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, WalletTypeEnum.I2.ToString());
                if (I2 == null)
                    throw new NotFoundException("I2 Wallet Not Found!!");

                I1.Balance -= realAmount;
                I1.UpdateBy = (Guid)accountTransaction.FromUserId;
                if ((await _investorWalletRepository.UpdateWalletBalance(I1)) == 0)
                    throw new CreateObjectException("Update I1 Wallet Balance Failed!!");
                

                //Create CASH_OUT WalletTransaction from I1 to I2
                walletTransaction.Description = "Transfer money from I1 wallet to I2 wallet";
                walletTransaction.FromWalletId = I1.Id;
                walletTransaction.ToWalletId = I2.Id;
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();

                if((await _walletTransactionRepository.CreateWalletTransaction(walletTransaction)).Equals(""))
                    throw new CreateObjectException("Investor Top Up Failed!!");

                I2.Balance += realAmount;
                I2.UpdateBy = I1.UpdateBy;
                if ((await _investorWalletRepository.UpdateWalletBalance(I2)) == 0)
                    throw new CreateObjectException("Update I2 Wallet Balance Failed!!");
                

                //Create CASH_IN WalletTransaction from I1 to I2
                walletTransaction.Description = "Receive money from I1 wallet to I2 wallet";
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                walletTransaction.CreateBy = I2.UpdateBy;

                if((await _walletTransactionRepository.CreateWalletTransaction(walletTransaction)).Equals(""))
                    throw new CreateObjectException("Investor Top Up Failed!!");
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

        public async void ProjectOwnerTopUp(AccountTransaction accountTransaction)
        {
            try
            {
                double realAmount = Convert.ToDouble(accountTransaction.Amount);
                ProjectWallet P1 = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType((Guid)accountTransaction.FromUserId, WalletTypeEnum.P1.ToString(), null);
                P1.Balance = realAmount;
                P1.UpdateBy = accountTransaction.FromUserId;

                int checkTopUp = await _projectWalletRepository.UpdateProjectWalletBalance(P1);
                if (checkTopUp == 0)
                    throw new CreateObjectException("Projert Owner Top Up Failed!!");

                //Create CASH_IN WalletTransaction to P1
                WalletTransaction walletTransaction = new()
                {
                    Amount = realAmount,
                    Fee = 0,
                    Description = "Deposit money into P1 wallet",
                    ToWalletId = P1.Id,
                    Type = WalletTransactionTypeEnum.DEPOSIT.ToString(),
                    CreateBy = accountTransaction.FromUserId
                };
                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                ProjectWallet P2 = await _projectWalletRepository.GetProjectWalletByProjectManagerIdAndType((Guid)accountTransaction.FromUserId, WalletTypeEnum.P2.ToString(), null);
                if (P2 == null)
                    throw new NotFoundException("P2 Wallet Not Found!!");

                P1.Balance = -realAmount;
                P1.UpdateBy = accountTransaction.FromUserId;
                int checkSuccess = await _projectWalletRepository.UpdateProjectWalletBalance(P1);
                if (checkSuccess == 0)
                    throw new CreateObjectException("Update P1 Wallet Balance Failed!!");

                //Create CASH_OUT WalletTransaction from P1 to P2
                walletTransaction.Description = "Transfer money from P1 wallet to P2 wallet";
                walletTransaction.FromWalletId = P1.Id;
                walletTransaction.ToWalletId = P2.Id;
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();

                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                P2.Balance = realAmount;
                P2.UpdateBy = P1.UpdateBy;
                checkSuccess = await _projectWalletRepository.UpdateProjectWalletBalance(P2);
                if (checkSuccess == 0)
                {
                    throw new CreateObjectException("Update P2 Wallet Balance Failed!!");
                }

                //Create CASH_IN WalletTransaction from P1 to P2
                walletTransaction.Description = "Receive money from P1 wallet to P2 wallet";
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                walletTransaction.CreateBy = P2.UpdateBy;

                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<string> CreateWithdrawAccountTransaction(dynamic wallet, WithdrawRequest withdrawRequest, string userId, string roleName)
        {
            try
            {
                string fromType = "";
                var arrayOfAllKeys = WalletTypeDictionary.walletTypes.Keys.ToArray();
                foreach (var key in arrayOfAllKeys)
                {
                    if (wallet.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault(key))))
                        fromType = key;
                    if (!fromType.Equals(""))
                        break;
                }
                Guid currentUserId = Guid.Parse(userId);
                wallet.Balance = -withdrawRequest.Amount;
                wallet.UpdateBy = currentUserId;
                if (roleName.Equals(RoleEnum.INVESTOR.ToString()))
                {
                    if (await _investorWalletRepository.UpdateInvestorWalletBalance(wallet) < 1)
                        throw new UpdateObjectException("Update Wallet Balance Failed!!");
                } else if (roleName.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                {
                    if (await _projectWalletRepository.UpdateProjectWalletBalance(wallet) < 1)
                        throw new UpdateObjectException("Update Wallet Balance Failed!!");
                }


                WalletTransaction walletTransaction = new()
                {
                    Amount = withdrawRequest.Amount,
                    Fee = 0,
                    Description = "Withdraw money out of "+fromType+" wallet",
                    Type = WalletTransactionTypeEnum.WITHDRAW.ToString(),
                    FromWalletId = wallet.Id,
                    CreateBy = currentUserId
                };
                string checkWalletTransaction = await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                if (checkWalletTransaction == null || checkWalletTransaction.Equals(""))
                    throw new CreateObjectException("Create Wallet Transaction Failed!!");

                AccountTransaction accountTransaction = new()
                {
                    FromUserId = currentUserId,
                    Amount = long.Parse(withdrawRequest.Amount.ToString()),
                    Message = "Giao dịch thành công.",
                    OrderType = "system",
                    PayType = "app",
                    ResultCode = 0,
                    Type = "WITHDRAW",
                    WithdrawRequestId = withdrawRequest.Id,
                    PartnerClientId = withdrawRequest.CreateBy
                };

                string result = await _accountTransactionRepository.CreateAccountTransaction(accountTransaction);
                if (result == null || result.Equals(""))
                    throw new CreateObjectException("Create Account Transaction Failed!!");
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }



        ////DELETE
        //public async Task<int> DeleteAccountTransactionById(Guid accountTransactionId)
        //{
        //    int result;
        //    try
        //    {

        //        result = await _accountTransactionRepository.DeleteAccountTransactionById(accountTransactionId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete AccountTransaction Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        ////GET ALL
        public async Task<AllAccountTransactionDTO> GetAllAccountTransactions(int pageIndex, int pageSize, string sort, ThisUserObj currentUser)
        {
            try
            {
                AllAccountTransactionDTO result = new AllAccountTransactionDTO();
                result.listOfAccountTransaction = new List<AccountTransactionDTO>();

                List<AccountTransaction> accountTransactionList = new();
                if (currentUser.roleId.Equals(currentUser.adminRoleId))
                {
                    accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions(pageIndex, pageSize, "", sort);
                    result.numOfAccountTransaction = await _accountTransactionRepository.CountAllAccountTransactions("");
                }
                else
                {
                    accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions(pageIndex, pageSize, currentUser.userId, sort);
                    result.numOfAccountTransaction = await _accountTransactionRepository.CountAllAccountTransactions(currentUser.userId);
                }


                result.listOfAccountTransaction = _mapper.Map<List<AccountTransactionDTO>>(accountTransactionList);
                foreach (AccountTransactionDTO item in result.listOfAccountTransaction)
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
        public async Task<AllAccountTransactionDTO> GetAccountTransactionsByDate(int pageIndex, int pageSize, string fromDate, string toDate, string sort, ThisUserObj currentUser)
        {
            try
            {
                AllAccountTransactionDTO result = new AllAccountTransactionDTO();
                result.listOfAccountTransaction = new List<AccountTransactionDTO>();

                if (fromDate == null || toDate == null)
                    throw new InvalidFieldException("fromDate and toDate can not be null!!");

                if (!await _validationService.CheckDate(fromDate))
                    throw new InvalidFieldException("Invalid fromDate!!!");

                if (!await _validationService.CheckDate(toDate))
                    throw new InvalidFieldException("Invalid toDate!!!");

                fromDate = fromDate.Remove(fromDate.Length - 8) + "00:00:00";
                toDate = toDate.Remove(toDate.Length - 8) + "23:59:59";

                if ((DateAndTime.DateDiff(DateInterval.Day, DateTime.ParseExact(fromDate, "dd/MM/yyyy HH:mm:ss", null), DateTime.ParseExact(toDate, "dd/MM/yyyy HH:mm:ss", null))) < 0)
                    throw new InvalidFieldException("fromDate can not bigger than toDate!!!");

                List<AccountTransaction> accountTransactionList = new();
                if (currentUser.roleId.Equals(currentUser.adminRoleId))
                {
                    accountTransactionList = await _accountTransactionRepository.GetAccountTransactionsByDate(pageIndex, pageSize, fromDate, toDate, sort, "");
                    result.numOfAccountTransaction = await _accountTransactionRepository.CountAccountTransactionsByDate(fromDate, toDate, "");
                }
                else
                {
                    accountTransactionList = await _accountTransactionRepository.GetAccountTransactionsByDate(pageIndex, pageSize, fromDate, toDate, sort, currentUser.userId);
                    result.numOfAccountTransaction = await _accountTransactionRepository.CountAccountTransactionsByDate(fromDate, toDate, currentUser.userId);
                }


                result.listOfAccountTransaction = _mapper.Map<List<AccountTransactionDTO>>(accountTransactionList);
                foreach (AccountTransactionDTO item in result.listOfAccountTransaction)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                }
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        ////GET BY ID
        //public async Task<AccountTransactionDTO> GetAccountTransactionById(Guid accountTransactionId)
        //{
        //    AccountTransactionDTO result;
        //    try
        //    {

        //        AccountTransaction dto = await _accountTransactionRepository.GetAccountTransactionById(accountTransactionId);
        //        result = _mapper.Map<AccountTransactionDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No AccountTransaction Object Found!");

        //        result.createDate = await _validationService.FormatDateOutput(result.createDate);
        //        result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        ////UPDATE
        //public async Task<int> UpdateAccountTransaction(AccountTransactionDTO accountTransactionDTO, Guid accountTransactionId)
        //{
        //    int result;
        //    try
        //    {
        //        if (accountTransactionDTO.fromUserId == null || !await _validationService.CheckUUIDFormat(accountTransactionDTO.fromUserId))
        //            throw new InvalidFieldException("Invalid fromUserId!!!");

        //        if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(accountTransactionDTO.fromUserId)))
        //            throw new NotFoundException("This fromUserId is not existed!!!");

        //        if (accountTransactionDTO.toUserId == null || !await _validationService.CheckUUIDFormat(accountTransactionDTO.toUserId))
        //            throw new InvalidFieldException("Invalid toUserId!!!");

        //        if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(accountTransactionDTO.toUserId)))
        //            throw new NotFoundException("This toUserId is not existed!!!");

        //        if (accountTransactionDTO.description != null && (accountTransactionDTO.description.Equals("string") || accountTransactionDTO.description.Length == 0))
        //            accountTransactionDTO.description = null;

        //        if (!await _validationService.CheckText(accountTransactionDTO.status))
        //            throw new InvalidFieldException("Invalid status!!!");

        //        if (accountTransactionDTO.createBy != null && accountTransactionDTO.createBy.Length >= 0)
        //        {
        //            if (accountTransactionDTO.createBy.Equals("string"))
        //                accountTransactionDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(accountTransactionDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (accountTransactionDTO.updateBy != null && accountTransactionDTO.updateBy.Length >= 0)
        //        {
        //            if (accountTransactionDTO.updateBy.Equals("string"))
        //                accountTransactionDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(accountTransactionDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        AccountTransaction dto = _mapper.Map<AccountTransaction>(accountTransactionDTO);
        //        result = await _accountTransactionRepository.UpdateAccountTransaction(dto, accountTransactionId);
        //        if (result == 0)
        //            throw new UpdateObjectException("Can not update AccountTransaction Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}
