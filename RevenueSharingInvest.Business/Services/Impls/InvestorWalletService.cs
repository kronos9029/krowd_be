using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public InvestorWalletService(IInvestorWalletRepository investorWalletRepository, IWalletTypeRepository walletTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _investorWalletRepository = investorWalletRepository;
            _walletTypeRepository = walletTypeRepository;
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
        //public async Task<InvestorWalletDTO> GetInvestorWalletById(Guid investorWalletId)
        //{
        //    InvestorWalletDTO result;
        //    try
        //    {
        //        InvestorWallet dto = await _investorWalletRepository.GetInvestorWalletById(investorWalletId);
        //        result = _mapper.Map<InvestorWalletDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No InvestorWallet Object Found!");

        //        result.createDate = await _validationService.FormatDateOutput(result.createDate);
        //        result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}
