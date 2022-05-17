using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
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
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IMapper _mapper;


        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, IMapper mapper)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO)
        {
            int result;
            try
            {
                WalletTransaction dto = _mapper.Map<WalletTransaction>(walletTransactionDTO);
                result = await _walletTransactionRepository.CreateWalletTransaction(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create WalletTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteWalletTransactionById(Guid walletTransactionId)
        {
            int result;
            try
            {

                result = await _walletTransactionRepository.DeleteWalletTransactionById(walletTransactionId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete WalletTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<WalletTransactionDTO>> GetAllWalletTransactions()
        {
            List<WalletTransaction> walletTransactionList = await _walletTransactionRepository.GetAllWalletTransactions();
            List<WalletTransactionDTO> list = _mapper.Map<List<WalletTransactionDTO>>(walletTransactionList);
            return list;
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
                    throw new CreateObjectException("No WalletTransaction Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletTransaction(WalletTransactionDTO walletTransactionDTO, Guid walletTransactionId)
        {
            int result;
            try
            {
                WalletTransaction dto = _mapper.Map<WalletTransaction>(walletTransactionDTO);
                result = await _walletTransactionRepository.UpdateWalletTransaction(dto, walletTransactionId);
                if (result == 0)
                    throw new CreateObjectException("Can not update WalletTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
