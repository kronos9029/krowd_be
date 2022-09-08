using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, IValidationService validationService, IMapper mapper)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllWalletTransactionData()
        {
            int result;
            try
            {
                result = await _walletTransactionRepository.ClearAllWalletTransactionData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (walletTransactionDTO.paymentId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.paymentId))
                    throw new InvalidFieldException("Invalid paymentId!!!");

                if (!await _validationService.CheckExistenceId("Payment", Guid.Parse(walletTransactionDTO.paymentId)))
                    throw new NotFoundException("This paymentId is not existed!!!");

                ///***

                if (walletTransactionDTO.systemWalletId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.systemWalletId))
                    throw new InvalidFieldException("Invalid systemWalletId!!!");

                if (!await _validationService.CheckExistenceId("SystemWallet", Guid.Parse(walletTransactionDTO.systemWalletId)))
                    throw new NotFoundException("This systemWalletId is not existed!!!");

                if (walletTransactionDTO.projectWalletId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.projectWalletId))
                    throw new InvalidFieldException("Invalid projectWalletId!!!");

                if (!await _validationService.CheckExistenceId("ProjectWallet", Guid.Parse(walletTransactionDTO.projectWalletId)))
                    throw new NotFoundException("This projectWalletId is not existed!!!");

                if (walletTransactionDTO.investorWalletId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.investorWalletId))
                    throw new InvalidFieldException("Invalid investorWalletId!!!");

                if (!await _validationService.CheckExistenceId("InvestorWallet", Guid.Parse(walletTransactionDTO.investorWalletId)))
                    throw new NotFoundException("This investorWalletId is not existed!!!");

                ///***

                if (walletTransactionDTO.amount <= 0)
                    throw new InvalidFieldException("amount must be greater than 0!!!");

                if (walletTransactionDTO.description != null && (walletTransactionDTO.description.Equals("string") || walletTransactionDTO.description.Length == 0))
                    walletTransactionDTO.description = null;

                if (!await _validationService.CheckText(walletTransactionDTO.type))
                    throw new InvalidFieldException("Invalid type!!!");

                //if (walletTransactionDTO.fromWalletId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.fromWalletId))
                //    throw new InvalidFieldException("Invalid fromWalletId!!!");

                //if (!await _validationService.CheckExistenceId("", Guid.Parse(walletTransactionDTO.fromWalletId)))
                //    throw new NotFoundException("This fromWalletId is not existed!!!");

                //if (walletTransactionDTO.projectWalletId == null || !await _validationService.CheckUUIDFormat(walletTransactionDTO.projectWalletId))
                //    throw new InvalidFieldException("Invalid projectWalletId!!!");

                //if (!await _validationService.CheckExistenceId("ProjectWallet", Guid.Parse(walletTransactionDTO.projectWalletId)))
                //    throw new NotFoundException("This projectWalletId is not existed!!!");

                WalletTransaction dto = _mapper.Map<WalletTransaction>(walletTransactionDTO);
                newId.id = await _walletTransactionRepository.CreateWalletTransaction(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create WalletTransaction Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete WalletTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<WalletTransactionDTO>> GetAllWalletTransactions(int pageIndex, int pageSize)
        {
            try
            {
                List<WalletTransaction> walletTransactionList = await _walletTransactionRepository.GetAllWalletTransactions(pageIndex, pageSize);
                List<WalletTransactionDTO> list = _mapper.Map<List<WalletTransactionDTO>>(walletTransactionList);

                foreach (WalletTransactionDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
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
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new UpdateObjectException("Can not update WalletTransaction Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
