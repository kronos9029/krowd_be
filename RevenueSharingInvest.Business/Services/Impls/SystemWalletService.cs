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
    public class SystemWalletService : ISystemWalletService
    {
        private readonly ISystemWalletRepository _systemWalletRepository;
        private readonly IMapper _mapper;


        public SystemWalletService(ISystemWalletRepository systemWalletRepository, IMapper mapper)
        {
            _systemWalletRepository = systemWalletRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateSystemWallet(SystemWalletDTO systemWalletDTO)
        {
            int result;
            try
            {
                SystemWallet dto = _mapper.Map<SystemWallet>(systemWalletDTO);
                result = await _systemWalletRepository.CreateSystemWallet(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create SystemWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not delete SystemWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<SystemWalletDTO>> GetAllSystemWallets()
        {
            List<SystemWallet> systemWalletList = await _systemWalletRepository.GetAllSystemWallets();
            List<SystemWalletDTO> list = _mapper.Map<List<SystemWalletDTO>>(systemWalletList);
            return list;
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
                    throw new CreateObjectException("No SystemWallet Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateSystemWallet(SystemWalletDTO systemWalletDTO, Guid systemWalletId)
        {
            int result;
            try
            {
                SystemWallet dto = _mapper.Map<SystemWallet>(systemWalletDTO);
                result = await _systemWalletRepository.UpdateSystemWallet(dto, systemWalletId);
                if (result == 0)
                    throw new CreateObjectException("Can not update SystemWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
