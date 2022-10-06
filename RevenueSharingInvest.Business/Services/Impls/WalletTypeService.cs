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
    public class WalletTypeService : IWalletTypeService
    {
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public WalletTypeService(IWalletTypeRepository walletTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _walletTypeRepository = walletTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //GET ALL
        public async Task<List<GetWalletTypeDTO>> GetAllWalletTypes()
        {
            try
            {
                List<WalletType> walletTypeList = await _walletTypeRepository.GetAllWalletTypes();
                List<GetWalletTypeDTO> list = _mapper.Map<List<GetWalletTypeDTO>>(walletTypeList);

                foreach (GetWalletTypeDTO item in list)
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
        public async Task<GetWalletTypeDTO> GetWalletTypeById(Guid walletTypeId)
        {
            GetWalletTypeDTO result;
            try
            {

                WalletType dto = await _walletTypeRepository.GetWalletTypeById(walletTypeId);
                result = _mapper.Map<GetWalletTypeDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No WalletType Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
