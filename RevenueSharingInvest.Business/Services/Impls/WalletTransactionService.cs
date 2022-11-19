using AutoMapper;
using Microsoft.VisualBasic;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IValidationService _validationService;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IMapper _mapper;

        public WalletTransactionService(
            IWalletTransactionRepository walletTransactionRepository,
            IInvestorWalletRepository investorWalletRepository,
            IWalletTypeRepository walletTypeRepository,
            IUserRepository userRepository,
            IProjectWalletRepository projectWalletRepository, 
            IValidationService validationService,
            IInvestorRepository investorRepository,
            IMapper mapper
            )
        {
            _walletTransactionRepository = walletTransactionRepository;
            _investorWalletRepository = investorWalletRepository;
            _walletTypeRepository = walletTypeRepository;
            _userRepository = userRepository;
            _projectWalletRepository = projectWalletRepository;
            _investorRepository = investorRepository;
            _validationService = validationService;
            _mapper = mapper;           
        }

        public async Task<string> TransferFromI1ToI2(ThisUserObj currentUser, double amount)
        {
            try
            {
                InvestorWallet I1 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I1.ToString());

                if (I1 == null)
                    throw new NotFoundException("I1 Wallet not Found!!");


                if (I1.Balance < amount || amount < 0)
                {
                    throw new WalletBalanceException("Insufficient Fund!!");
                }
                else
                {
                    InvestorWallet I2 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(Guid.Parse(currentUser.investorId), WalletTypeEnum.I2.ToString());
                    if (I2 == null)
                        throw new NotFoundException("I2 Wallet Not Found!!");

                    I1.Balance -= amount;
                    I1.UpdateBy = Guid.Parse(currentUser.userId);
                    int checkSuccess = await _investorWalletRepository.UpdateWalletBalance(I1);
                    if (checkSuccess == 0)
                    {
                        throw new CreateObjectException("Update I1 Wallet Balance Failed!!");
                    }
                    I2.Balance += amount;
                    I2.UpdateBy = I1.UpdateBy;
                    checkSuccess = await _investorWalletRepository.UpdateWalletBalance(I2);
                    if (checkSuccess == 0)
                    {
                        throw new CreateObjectException("Update I2 Wallet Balance Failed!!");
                    }

                    WalletTransaction walletTransaction = new();

                    walletTransaction.Amount = amount;
                    walletTransaction.Description = "Investor Transfer Money From I1 Wallet To I2 Wallet";
                    walletTransaction.FromWalletId = I1.Id;
                    walletTransaction.ToWalletId = I2.Id;
                    walletTransaction.Type = "Top-up";
                    walletTransaction.CreateBy = Guid.Parse(currentUser.investorId);

                    string transactionId = await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                    if (transactionId == null)
                    {
                        throw new CreateObjectException("Create Wallet Transaction Failed!!");
                    }

                    return transactionId;

                }
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async void TransferMoney(dynamic from, dynamic to, double amount, string userId, string action)
        {
            try
            {
                if (amount < 0 || amount > from.Balance)
                    throw new WalletBalanceException("Invalid Amount!!");
                var arrayOfAllKeys = WalletTypeDictionary.walletTypes.Keys.ToArray();

                string fromType = "";
                string toType = "";

                foreach (var key in arrayOfAllKeys)
                {
                    if (from.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault(key))))
                        fromType = key;
                    if (to.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault(key))))
                        toType = key;
                    if (!fromType.Equals("") && !toType.Equals(""))
                        break;
                }

                //Subtract  balance
                from.Balance = -amount;
                from.UpdateBy = Guid.Parse(userId);
                if (fromType.Contains("I"))
                {
                    await _investorWalletRepository.UpdateInvestorWalletBalance(from);
                } else
                {
                    await _projectWalletRepository.UpdateProjectWalletBalance(from);
                }


                //Create CASH_OUT WalletTransaction
                WalletTransaction walletTransaction = new()
                {
                    Amount = amount,
                    Fee = 0,
                    Description = "Transfer money from " + fromType + " wallet to " + toType + " wallet",
                    FromWalletId = from.Id,
                    ToWalletId = to.Id,
                    Type = WalletTransactionTypeEnum.CASH_OUT.ToString(),
                    CreateBy = Guid.Parse(userId)
                };
                if (action.Equals("REFUND"))
                {
                    walletTransaction.Description = "Refund money from withdraw request";
                }
                string newWalletTransactionId = await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                if (newWalletTransactionId == null || newWalletTransactionId.Equals(""))
                    throw new CreateObjectException("Create Wallet Transaction Failed!!");

                //Add to balance
                to.Balance = amount;
                if (toType.Contains("I"))
                {
                    await _investorWalletRepository.UpdateInvestorWalletBalance(to);
                }
                else
                {
                    await _projectWalletRepository.UpdateProjectWalletBalance(to);
                }


                //Create CASH_IN WalletTransaction
                walletTransaction.Description = "Receive money from " + fromType + " wallet to " + toType + " wallet";
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                newWalletTransactionId = "";
                newWalletTransactionId = await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                if (newWalletTransactionId == null || newWalletTransactionId.Equals(""))
                    throw new CreateObjectException("Create Wallet Transaction Failed!!");
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<AllWalletTransactionDTO> GetAllWalletTransactions(int pageIndex, int pageSize, Guid? userId, Guid? walletId, string fromDate, string toDate, string type, string order, ThisUserObj currentUser)
        {
            AllWalletTransactionDTO result = new AllWalletTransactionDTO();
            result.listOfWalletTransaction = new List<WalletTransactionDTO>();
            result.filterCount = new CountWalletTransactionDTO();
            Guid? userRoleId = null;
            try
            {
                if (userId == null)
                {
                    if (currentUser.roleId.Equals(currentUser.adminRoleId))
                        throw new InvalidFieldException("userId can not be null!!!");
                }  
                else
                {
                    if (!await _validationService.CheckExistenceId("[User]", (Guid)userId))
                        throw new NotFoundException("This userId is not existed!!!");

                    User user = await _userRepository.GetUserById((Guid)userId);
                    if (!user.RoleId.Equals(Guid.Parse(currentUser.projectManagerRoleId)) && !user.RoleId.Equals(Guid.Parse(currentUser.investorRoleId)))
                        throw new InvalidFieldException("You can view WalletTransaction of PROJECT_MANAGER or INVESTOR only!!!");

                    if ((currentUser.roleId.Equals(currentUser.projectManagerRoleId) || currentUser.roleId.Equals(currentUser.investorRoleId)) && !userId.Equals(Guid.Parse(currentUser.userId)))
                        throw new InvalidFieldException("This userId is not your userId!!!");
                }

                if (walletId != null)
                {
                    if (!await _validationService.CheckExistenceId("ProjectWallet", (Guid)walletId) && !await _validationService.CheckExistenceId("InvestorWallet", (Guid)walletId))
                        throw new NotFoundException("This walletId is not existed!!!");

                    if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
                    {
                        ProjectWallet projectWallet = await _projectWalletRepository.GetProjectWalletById((Guid)walletId);
                        if (!projectWallet.ProjectManagerId.Equals(Guid.Parse(currentUser.userId)))
                            throw new InvalidFieldException("This walletId is not your walletId!!!");
                    }
                    else if (currentUser.roleId.Equals(currentUser.investorRoleId))
                    {
                        InvestorWallet investorWallet = await _investorWalletRepository.GetInvestorWalletById((Guid)walletId);
                        if (!investorWallet.InvestorId.Equals(Guid.Parse(currentUser.investorId)))
                            throw new InvalidFieldException("This walletId is not your walletId!!!");
                    }

                }

                if ((fromDate != null || toDate != null) && (fromDate == null || toDate == null))
                    throw new InvalidFieldException("fromDate and toDate must be used at the same time!!!");

                if (fromDate != null && toDate != null)
                {
                    if (!await _validationService.CheckDate((fromDate)))
                        throw new InvalidFieldException("Invalid fromDate!!!");

                    if (!await _validationService.CheckDate((toDate)))
                        throw new InvalidFieldException("Invalid toDate!!!");

                    if ((DateAndTime.DateDiff(DateInterval.Day, DateTime.ParseExact(fromDate, "dd/MM/yyyy HH:mm:ss", null), DateTime.ParseExact(toDate, "dd/MM/yyyy HH:mm:ss", null))) < 0)
                        throw new InvalidFieldException("startDate can not bigger than endDate!!!");

                    fromDate = fromDate.Remove(fromDate.Length - 8) + "00:00:00";
                    toDate = toDate.Remove(toDate.Length - 8) + "23:59:59";
                }

                if (type != null && !type.Equals(WalletTransactionTypeEnum.CASH_IN.ToString()) 
                    && !type.Equals(WalletTransactionTypeEnum.CASH_OUT.ToString())
                    && !type.Equals(WalletTransactionTypeEnum.DEPOSIT.ToString()) 
                    && !type.Equals(WalletTransactionTypeEnum.WITHDRAW.ToString()))
                    throw new InvalidFieldException("type must be CASH_IN or CASH_OUT or DEPOSIT or WITHDRAW!!!");

                if (order != null && !order.Equals(OrderEnum.ASC.ToString()) && !order.Equals(OrderEnum.DESC.ToString()))
                    throw new InvalidFieldException("order must be ASC or DESC!!!");

                userId = (currentUser.roleId.Equals(currentUser.adminRoleId)) ? userId : Guid.Parse(currentUser.userId);
                userRoleId = (await _userRepository.GetUserById((Guid)userId)).RoleId;

                List<WalletTransaction> walletTransactionsEntityList = await _walletTransactionRepository.GetAllWalletTransactions(pageIndex, pageSize, userId, userRoleId, walletId, fromDate, toDate, type, order);
                result.listOfWalletTransaction = _mapper.Map<List<WalletTransactionDTO>>(walletTransactionsEntityList);
                result.numOfWalletTransaction = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, walletId, fromDate, toDate, type);
                foreach (WalletTransactionDTO item in result.listOfWalletTransaction)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                }

                //Filter count
                result.filterCount.all = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, null, null, null, null);

                dynamic wallets = userRoleId.Equals(Guid.Parse(currentUser.investorRoleId))
                    ? await _investorWalletRepository.GetInvestorWalletsByInvestorId((await _investorRepository.GetInvestorByUserId((Guid)userId)).Id)
                    : await _projectWalletRepository.GetProjectWalletsByProjectManagerId((Guid)userId);

                foreach (dynamic item in wallets)
                {
                    if (userRoleId.Equals(Guid.Parse(currentUser.investorRoleId)))
                    {
                        if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I1"))))
                            result.filterCount.i1 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                        
                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2"))))
                            result.filterCount.i2 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                        
                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"))))
                            result.filterCount.i3 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                        
                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4"))))
                            result.filterCount.i4 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                        
                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5"))))
                            result.filterCount.i5 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                    }
                    else if (userRoleId.Equals(Guid.Parse(currentUser.projectManagerRoleId)))
                    {
                        if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P1"))))
                            result.filterCount.p1 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);

                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2"))))
                            result.filterCount.p2 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);

                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P3"))))
                            result.filterCount.p3 = result.filterCount.p3 == null ? 0 : result.filterCount.p3 + await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);

                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4"))))
                            result.filterCount.p4 = result.filterCount.p4 == null ? 0 : result.filterCount.p4 + await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);

                        else if (item.WalletTypeId.Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5"))))
                            result.filterCount.p5 = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, item.Id, null, null, null);
                    }
                }

                result.filterCount.cashIn = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, null, null, null, WalletTransactionTypeEnum.CASH_IN.ToString());
                result.filterCount.cashOut = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, null, null, null, WalletTransactionTypeEnum.CASH_OUT.ToString());
                result.filterCount.deposit = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, null, null, null, WalletTransactionTypeEnum.DEPOSIT.ToString());
                result.filterCount.withdraw = await _walletTransactionRepository.CountAllWalletTransactions(userId, userRoleId, null, null, null, WalletTransactionTypeEnum.WITHDRAW.ToString());

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        ////GET BY ID
        //public async Task<WalletTransactionDTO> GetWalletTransactionById(Guid walletTransactionId)
        //{
        //    WalletTransactionDTO result;
        //    try
        //    {
        //        WalletTransaction dto = await _walletTransactionRepository.GetWalletTransactionById(walletTransactionId);
        //        result = _mapper.Map<WalletTransactionDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No WalletTransaction Object Found!");

        //        result.createDate = await _validationService.FormatDateOutput(result.createDate);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Logger(e.ToString());
        //        throw new Exception(e.Message);
        //    }
        //}

        public async Task<List<WalletType>> GetUserWallet(string Mode)
        {
            try
            {
                List<WalletType> walletTypes = await _walletTypeRepository.GetWalletByMode(RoleEnum.INVESTOR.ToString());
                return walletTypes;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
