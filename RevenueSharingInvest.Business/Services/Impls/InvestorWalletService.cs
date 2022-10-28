using AutoMapper;
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class InvestorWalletService : IInvestorWalletService
    {
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public InvestorWalletService(
            IInvestorWalletRepository investorWalletRepository, 
            IWalletTypeRepository walletTypeRepository, 
            IInvestorRepository investorRepository,
            IWalletTransactionRepository walletTransactionRepository,

            IValidationService validationService, 
            
            IMapper mapper)
        {
            _investorWalletRepository = investorWalletRepository;
            _walletTypeRepository = walletTypeRepository;
            _investorRepository = investorRepository;
            _walletTransactionRepository = walletTransactionRepository;

            _validationService = validationService;
            _mapper = mapper;
        }

        //GET ALL
        public async Task<UserWalletsDTO> GetAllInvestorWallets(ThisUserObj currentUser)
        {
            try
            {
                UserWalletsDTO result = new UserWalletsDTO();
                result.listOfInvestorWallet = new List<GetInvestorWalletDTO>();
                List<InvestorWallet> investorWalletList = await _investorWalletRepository.GetInvestorWalletsByInvestorId(Guid.Parse(currentUser.investorId));
                List<MappedInvestorWalletDTO> list = _mapper.Map<List<MappedInvestorWalletDTO>>(investorWalletList);
                GetInvestorWalletDTO dto = new GetInvestorWalletDTO();

                foreach (MappedInvestorWalletDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    dto = _mapper.Map<GetInvestorWalletDTO>(item);
                    dto.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById(Guid.Parse(item.walletTypeId)));

                    result.totalAsset += (float)item.balance;
                    result.listOfInvestorWallet.Add(dto);
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetInvestorWalletDTO> GetInvestorWalletById(Guid id, ThisUserObj currentUser)
        {
            GetInvestorWalletDTO result;
            try
            {
                InvestorWallet investorWallet = await _investorWalletRepository.GetInvestorWalletById(id);
                if (investorWallet == null)
                    throw new NotFoundException("No InvestorWallet Object Found!");
                if (!currentUser.investorId.Equals(investorWallet.InvestorId.ToString()))
                    throw new InvalidFieldException("This id is not belong to your wallets!!!");

                result = _mapper.Map<GetInvestorWalletDTO>(investorWallet);
                result.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById((Guid)investorWallet.WalletTypeId));
                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        //TRANSFER MONEY
        public async Task<TransferResponseDTO> TransferBetweenInvestorWallet(TransferDTO transferDTO, ThisUserObj currentUser)
        {
            TransferResponseDTO result = new TransferResponseDTO();
            try
            {
                InvestorWallet fromWallet = await _investorWalletRepository.GetInvestorWalletById(transferDTO.fromWalletId);
                WalletType fromWalletType = await _walletTypeRepository.GetWalletTypeById((Guid)fromWallet.WalletTypeId);
                InvestorWallet toWallet = await _investorWalletRepository.GetInvestorWalletById(transferDTO.toWalletId);
                WalletType toWalletType = await _walletTypeRepository.GetWalletTypeById((Guid)toWallet.WalletTypeId);

                if (fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4")) 
                    && !fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5")))
                    throw new InvalidFieldException("You can transfer money from I4 to I5 only!!!");
                else if (fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5")) 
                    && !fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")))
                    throw new InvalidFieldException("You can transfer money from I5 to I2 only!!!");

                if (fromWallet.Balance < transferDTO.amount) throw new InvalidFieldException("Your wallet balance is not enough to transfer!!!");

                //Subtract fromWallet balance
                InvestorWallet investorWallet = new InvestorWallet();
                investorWallet.InvestorId = Guid.Parse(currentUser.investorId);
                investorWallet.WalletTypeId = fromWallet.WalletTypeId;
                investorWallet.Balance = -transferDTO.amount;
                investorWallet.UpdateBy = Guid.Parse(currentUser.userId);
                await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                //Create CASH_OUT WalletTransaction from fromWallet to toWallet
                WalletTransaction walletTransaction = new WalletTransaction();
                walletTransaction.Amount = transferDTO.amount;
                walletTransaction.Fee = 0;
                walletTransaction.Description = "Transfer money from " + fromWalletType.Type + " to " + toWalletType.Type;
                walletTransaction.FromWalletId = transferDTO.fromWalletId;
                walletTransaction.ToWalletId = transferDTO.toWalletId;
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                //Add toWallet balance
                investorWallet.WalletTypeId = toWallet.WalletTypeId;
                investorWallet.Balance = transferDTO.amount;
                await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet);

                //Create CASH_IN WalletTransaction fromWallet to toWallet
                walletTransaction.Description = "Receive money from " + fromWalletType.Type + " to " + toWalletType.Type;
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                //Mapping response
                result.fromWalletId = fromWallet.Id;
                result.fromWalletName = fromWalletType.Name;
                result.fromWalletType = fromWalletType.Type;
                result.toWalletId = toWallet.Id;
                result.toWalletName = toWalletType.Name;
                result.toWalletType = toWalletType.Type;
                result.amount = transferDTO.amount;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
