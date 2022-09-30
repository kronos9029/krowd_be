using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
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
        private readonly IMapper _mapper;


        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, 
            IValidationService validationService, 
            IMapper mapper,
            IInvestorWalletRepository investorWalletRepository,
            IWalletTypeRepository walletTypeRepository)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _validationService = validationService;
            _mapper = mapper;
            _investorWalletRepository = investorWalletRepository;
            _walletTypeRepository = walletTypeRepository;
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

        //GET ALL
        public async Task<List<WalletTransactionDTO>> GetAllWalletTransactions(int pageIndex, int pageSize, string sort,string fromDate, string toDate, string userId, string walletId)
        {
            fromDate ??= "";
            toDate ??= "";
            walletId ??= "";
            try
            {
                if(!fromDate.Equals("") || !toDate.Equals(""))
                {
                    fromDate += " 00:00:00";
                    toDate += " 23:59:59";
                }

                List<WalletTransaction> walletTransactionList = await _walletTransactionRepository.GetAllWalletTransactions(pageIndex, pageSize,userId,sort, fromDate, toDate, walletId);
                List<WalletTransactionDTO> list = _mapper.Map<List<WalletTransactionDTO>>(walletTransactionList);

                foreach (WalletTransactionDTO item in list)
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

        //GET BY ID
        public async Task<WalletTransactionDTO> GetWalletTransactionById(Guid walletTransactionId)
        {
            WalletTransactionDTO result;
            try
            {
                WalletTransaction dto = await _walletTransactionRepository.GetWalletTransactionById(walletTransactionId);
                result = _mapper.Map<WalletTransactionDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No WalletTransaction Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

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
