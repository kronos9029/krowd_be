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
    public class SystemWalletService : ISystemWalletService
    {
        private readonly ISystemWalletRepository _systemWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public SystemWalletService(ISystemWalletRepository systemWalletRepository, IValidationService validationService, IMapper mapper)
        {
            _systemWalletRepository = systemWalletRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllSystemWalletData()
        {
            int result;
            try
            {
                result = await _systemWalletRepository.ClearAllSystemWalletData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateSystemWallet(SystemWalletDTO systemWalletDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                systemWalletDTO.balance = 0;

                if (systemWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(systemWalletDTO.walletTypeId))
                    throw new InvalidFieldException("Invalid walletTypeId!!!");

                if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(systemWalletDTO.walletTypeId)))
                    throw new NotFoundException("This walletTypeId is not existed!!!");

                if (systemWalletDTO.createBy != null && systemWalletDTO.createBy.Length >= 0)
                {
                    if (systemWalletDTO.createBy.Equals("string"))
                        systemWalletDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(systemWalletDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (systemWalletDTO.updateBy != null && systemWalletDTO.updateBy.Length >= 0)
                {
                    if (systemWalletDTO.updateBy.Equals("string"))
                        systemWalletDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(systemWalletDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                systemWalletDTO.isDeleted = false;

                SystemWallet dto = _mapper.Map<SystemWallet>(systemWalletDTO);
                newId.id = await _systemWalletRepository.CreateSystemWallet(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create SystemWallet Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteSystemWalletById(Guid systemWalletId)
        {
            int result;
            try
            {

                result = await _systemWalletRepository.DeleteSystemWalletById(systemWalletId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete SystemWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<SystemWalletDTO>> GetAllSystemWallets(int pageIndex, int pageSize)
        {
            try
            {
                List<SystemWallet> systemWalletList = await _systemWalletRepository.GetAllSystemWallets(pageIndex, pageSize);
                List<SystemWalletDTO> list = _mapper.Map<List<SystemWalletDTO>>(systemWalletList);

                foreach (SystemWalletDTO item in list)
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
        public async Task<SystemWalletDTO> GetSystemWalletById(Guid systemWalletId)
        {
            SystemWalletDTO result;
            try
            {

                SystemWallet dto = await _systemWalletRepository.GetSystemWalletById(systemWalletId);
                result = _mapper.Map<SystemWalletDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No SystemWallet Object Found!");

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
        public async Task<int> UpdateSystemWallet(SystemWalletDTO systemWalletDTO, Guid systemWalletId)
        {
            int result;
            try
            {
                if (systemWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(systemWalletDTO.walletTypeId))
                    throw new InvalidFieldException("Invalid walletTypeId!!!");

                if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(systemWalletDTO.walletTypeId)))
                    throw new NotFoundException("This walletTypeId is not existed!!!");

                if (systemWalletDTO.createBy != null && systemWalletDTO.createBy.Length >= 0)
                {
                    if (systemWalletDTO.createBy.Equals("string"))
                        systemWalletDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(systemWalletDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (systemWalletDTO.updateBy != null && systemWalletDTO.updateBy.Length >= 0)
                {
                    if (systemWalletDTO.updateBy.Equals("string"))
                        systemWalletDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(systemWalletDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                SystemWallet dto = _mapper.Map<SystemWallet>(systemWalletDTO);
                result = await _systemWalletRepository.UpdateSystemWallet(dto, systemWalletId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update SystemWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
