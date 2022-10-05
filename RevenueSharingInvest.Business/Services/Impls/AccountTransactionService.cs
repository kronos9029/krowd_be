using AutoMapper;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public AccountTransactionService(IAccountTransactionRepository accountTransactionRepository, 
            IValidationService validationService, 
            IMapper mapper, 
            IInvestorWalletRepository investorWalletRepository,
            IWalletTypeRepository walletTypeRepository,
            IInvestorRepository investorRepository,
            IWalletTransactionRepository walletTransactionRepository)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _investorWalletRepository = investorWalletRepository;
            _validationService = validationService;
            _walletTypeRepository = walletTypeRepository;
            _mapper = mapper;
            _investorRepository = investorRepository;
            _walletTransactionRepository = walletTransactionRepository;
        }

        //CREATE
        public async Task<IdDTO> CreateAccountTransaction(MomoPaymentResult momoPaymentResult)
        {
            IdDTO newId = new IdDTO();
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

                //accountTransactionDTO.isDeleted = false;

                AccountTransaction entity = _mapper.Map<AccountTransaction>(momoPaymentResult);
                entity.PartnerClientId = Guid.Parse(momoPaymentResult.partnerClientId);
                entity.FromUserId = entity.PartnerClientId;
                entity.Type = "TOP-UP";
                newId.id = await _accountTransactionRepository.CreateAccountTransaction(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create AccountTransaction Object!");
                Investor investor = await _investorRepository.GetInvestorByUserId((Guid)entity.PartnerClientId);

                if(entity.ResultCode == 0)
                {
                    //investor top-up I1 Wallet
                    double realAmount = Convert.ToDouble(entity.Amount);
                    InvestorWallet I1 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, WalletTypeEnum.I1.ToString());
                    I1.Balance += realAmount;
                    I1.UpdateDate = DateTime.Now;
                    I1.UpdateBy = entity.FromUserId;
                    
                    int checkTopUp = await _investorWalletRepository.UpdateWalletBalance(I1);
                    WalletTransaction walletTransaction = new WalletTransaction();
                    if (checkTopUp == 0)
                    {
                        throw new CreateObjectException("Investor Top Up Failed!!");
                    }
                    else
                    {
                        //Create CASH_IN WalletTransaction to I1
                        walletTransaction = new WalletTransaction();

                        walletTransaction.Amount = realAmount;
                        walletTransaction.Fee = 0;
                        walletTransaction.Description = "Deposit money into I1 wallet";
                        walletTransaction.ToWalletId = I1.Id;
                        walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                        walletTransaction.CreateBy = entity.FromUserId;

                        await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);
                    }

                    //Money From I1 Wallet automaticly tranfer from I1 to I2
                    InvestorWallet I2 = await _investorWalletRepository.GetInvestorWalletByInvestorIdAndType(investor.Id, WalletTypeEnum.I2.ToString());
                    if (I2 == null)
                        throw new NotFoundException("I2 Wallet Not Found!!");

                    I1.UpdateDate = DateTime.Now;
                    I1.Balance -= realAmount;
                    I1.UpdateBy = entity.FromUserId;
                    int checkSuccess = await _investorWalletRepository.UpdateWalletBalance(I1);
                    if (checkSuccess == 0)
                    {
                        throw new CreateObjectException("Update I1 Wallet Balance Failed!!");
                    }

                    //Create CASH_OUT WalletTransaction from I1 to I2
                    walletTransaction.Description = "Transfer money from I1 wallet to I2 wallet";
                    walletTransaction.FromWalletId = I1.Id;
                    walletTransaction.ToWalletId = I2.Id;
                    walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();

                    await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                    I2.Balance += realAmount;
                    I2.UpdateBy = I1.UpdateBy;
                    checkSuccess = await _investorWalletRepository.UpdateWalletBalance(I2);
                    if (checkSuccess == 0)
                    {
                        throw new CreateObjectException("Update I2 Wallet Balance Failed!!");
                    }

                    //Create CASH_IN WalletTransaction from I1 to I2
                    walletTransaction.Description = "Receive money from I1 wallet to I2 wallet";
                    walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                    walletTransaction.CreateBy = I2.UpdateBy;

                    await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                }

                return newId;
            }
            catch (Exception e)
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
        public async Task<List<AccountTransactionDTO>> GetAllAccountTransactions(int pageIndex, int pageSize, string sort, ThisUserObj currentUser)
        {
            try
            {
                List<AccountTransaction> accountTransactionList = new();
                if (currentUser.roleId.Equals(currentUser.adminRoleId))
                {
                    accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions(pageIndex, pageSize, "", sort);
                }
                else
                {
                    accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions(pageIndex, pageSize, currentUser.userId, sort);
                }

                
                List<AccountTransactionDTO> list = _mapper.Map<List<AccountTransactionDTO>>(accountTransactionList);
                foreach (AccountTransactionDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                }
                return list;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
        public async Task<List<AccountTransactionDTO>> GetAccountTransactionsByDate(int pageIndex, int pageSize, string fromDate, string toDate, string sort, ThisUserObj currentUser)
        {
            try
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";
                List<AccountTransaction> accountTransactionList = new();
                if (currentUser.roleId.Equals(currentUser.adminRoleId))
                {
                    accountTransactionList = await _accountTransactionRepository.GetAccountTransactionsByDate(pageIndex, pageSize, fromDate, toDate, sort, "");
                }
                else
                {
                    accountTransactionList = await _accountTransactionRepository.GetAccountTransactionsByDate(pageIndex, pageSize, fromDate, toDate, sort, currentUser.userId);
                }


                List<AccountTransactionDTO> list = _mapper.Map<List<AccountTransactionDTO>>(accountTransactionList);
                foreach (AccountTransactionDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                }
                return list;
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
