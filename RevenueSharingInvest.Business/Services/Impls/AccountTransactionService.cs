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
    public class AccountTransactionService : IAccountTransactionService
    {
        private readonly IAccountTransactionRepository _accountTransactionRepository;
        private readonly IMapper _mapper;


        public AccountTransactionService(IAccountTransactionRepository accountTransactionRepository, IMapper mapper)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateAccountTransaction(AccountTransactionDTO accountTransactionDTO)
        {
            int result;
            try
            {
                AccountTransaction dto = _mapper.Map<AccountTransaction>(accountTransactionDTO);
                result = await _accountTransactionRepository.CreateAccountTransaction(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create AccountTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteAccountTransactionById(Guid accountTransactionId)
        {
            int result;
            try
            {

                result = await _accountTransactionRepository.DeleteAccountTransactionById(accountTransactionId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete AccountTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<AccountTransactionDTO>> GetAllAccountTransactions()
        {
            List<AccountTransaction> accountTransactionList = await _accountTransactionRepository.GetAllAccountTransactions();
            List<AccountTransactionDTO> list = _mapper.Map<List<AccountTransactionDTO>>(accountTransactionList);
            return list;
        }

        //GET BY ID
        public async Task<AccountTransactionDTO> GetAccountTransactionById(Guid accountTransactionId)
        {
            AccountTransactionDTO result;
            try
            {

                AccountTransaction dto = await _accountTransactionRepository.GetAccountTransactionById(accountTransactionId);
                result = _mapper.Map<AccountTransactionDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No AccountTransaction Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateAccountTransaction(AccountTransactionDTO accountTransactionDTO, Guid accountTransactionId)
        {
            int result;
            try
            {
                AccountTransaction dto = _mapper.Map<AccountTransaction>(accountTransactionDTO);
                result = await _accountTransactionRepository.UpdateAccountTransaction(dto, accountTransactionId);
                if (result == 0)
                    throw new CreateObjectException("Can not update AccountTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
