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
    public class WalletTypeService : IWalletTypeService
    {
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IMapper _mapper;


        public WalletTypeService(IWalletTypeRepository walletTypeRepository, IMapper mapper)
        {
            _walletTypeRepository = walletTypeRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateWalletType(WalletTypeDTO walletTypeDTO)
        {
            int result;
            try
            {
                WalletType dto = _mapper.Map<WalletType>(walletTypeDTO);
                result = await _walletTypeRepository.CreateWalletType(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create WalletType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteWalletTypeById(Guid walletTypeId)
        {
            int result;
            try
            {

                result = await _walletTypeRepository.DeleteWalletTypeById(walletTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete WalletType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<WalletTypeDTO>> GetAllWalletTypes()
        {
            List<WalletType> walletTypeList = await _walletTypeRepository.GetAllWalletTypes();
            List<WalletTypeDTO> list = _mapper.Map<List<WalletTypeDTO>>(walletTypeList);
            return list;
        }

        //GET BY ID
        public async Task<WalletTypeDTO> GetWalletTypeById(Guid walletTypeId)
        {
            WalletTypeDTO result;
            try
            {

                WalletType dto = await _walletTypeRepository.GetWalletTypeById(walletTypeId);
                result = _mapper.Map<WalletTypeDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No WalletType Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletType(WalletTypeDTO walletTypeDTO, Guid walletTypeId)
        {
            int result;
            try
            {
                WalletType dto = _mapper.Map<WalletType>(walletTypeDTO);
                result = await _walletTypeRepository.UpdateWalletType(dto, walletTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not update WalletType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
