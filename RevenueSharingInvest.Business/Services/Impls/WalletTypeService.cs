using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
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

        //CLEAR DATA
        public async Task<int> ClearAllWalletTypeData()
        {
            int result;
            try
            {
                result = await _walletTypeRepository.ClearAllWalletTypeData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateWalletType(WalletTypeDTO walletTypeDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(walletTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (walletTypeDTO.description != null && (walletTypeDTO.description.Equals("string") || walletTypeDTO.description.Length == 0))
                    walletTypeDTO.description = null;

                if(!await _validationService.CheckText(walletTypeDTO.mode) || (!walletTypeDTO.mode.Equals("INVESTOR") && !walletTypeDTO.mode.Equals("BUSINESS") && !walletTypeDTO.mode.Equals("SYSTEM")))
                    throw new InvalidFieldException("Mode must be 'INVESTOR' or 'BUSINESS' or 'SYSTEM'!!!");

                if (!await _validationService.CheckText(walletTypeDTO.type))
                    throw new InvalidFieldException("Invalid type!!!");

                if (walletTypeDTO.createBy != null && walletTypeDTO.createBy.Length >= 0)
                {
                    if (walletTypeDTO.createBy.Equals("string"))
                        walletTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(walletTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (walletTypeDTO.updateBy != null && walletTypeDTO.updateBy.Length >= 0)
                {
                    if (walletTypeDTO.updateBy.Equals("string"))
                        walletTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(walletTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                walletTypeDTO.isDeleted = false;

                WalletType dto = _mapper.Map<WalletType>(walletTypeDTO);
                newId.id = await _walletTypeRepository.CreateWalletType(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create WalletType Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete WalletType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<WalletTypeDTO>> GetAllWalletTypes()
        {
            try
            {
                List<WalletType> walletTypeList = await _walletTypeRepository.GetAllWalletTypes();
                List<WalletTypeDTO> list = _mapper.Map<List<WalletTypeDTO>>(walletTypeList);
                return list;
            }           
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No WalletType Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletType(WalletTypeDTO walletTypeDTO, Guid walletTypeId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(walletTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (walletTypeDTO.description != null && (walletTypeDTO.description.Equals("string") || walletTypeDTO.description.Length == 0))
                    walletTypeDTO.description = null;

                if (!await _validationService.CheckText(walletTypeDTO.mode) || (!walletTypeDTO.mode.Equals("INVESTOR") && !walletTypeDTO.mode.Equals("BUSINESS") && !walletTypeDTO.mode.Equals("SYSTEM")))
                    throw new InvalidFieldException("Mode must be 'INVESTOR' or 'BUSINESS' or 'SYSTEM'!!!");

                if (!await _validationService.CheckText(walletTypeDTO.type))
                    throw new InvalidFieldException("Invalid type!!!");

                if (walletTypeDTO.createBy != null && walletTypeDTO.createBy.Length >= 0)
                {
                    if (walletTypeDTO.createBy.Equals("string"))
                        walletTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(walletTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (walletTypeDTO.updateBy != null && walletTypeDTO.updateBy.Length >= 0)
                {
                    if (walletTypeDTO.updateBy.Equals("string"))
                        walletTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(walletTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                WalletType dto = _mapper.Map<WalletType>(walletTypeDTO);
                result = await _walletTypeRepository.UpdateWalletType(dto, walletTypeId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update WalletType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
