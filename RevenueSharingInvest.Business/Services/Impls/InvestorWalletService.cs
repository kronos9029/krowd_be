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
    public class InvestorWalletService : IInvestorWalletService
    {
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IMapper _mapper;


        public InvestorWalletService(IInvestorWalletRepository investorWalletRepository, IMapper mapper)
        {
            _investorWalletRepository = investorWalletRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateInvestorWallet(InvestorWalletDTO investorWalletDTO)
        {
            int result;
            try
            {
                InvestorWallet dto = _mapper.Map<InvestorWallet>(investorWalletDTO);
                result = await _investorWalletRepository.CreateInvestorWallet(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create InvestorWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteInvestorWalletById(Guid investorWalletId)
        {
            int result;
            try
            {

                result = await _investorWalletRepository.DeleteInvestorWalletById(investorWalletId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete InvestorWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<InvestorWalletDTO>> GetAllInvestorWallets()
        {
            List<InvestorWallet> investorWalletList = await _investorWalletRepository.GetAllInvestorWallets();
            List<InvestorWalletDTO> list = _mapper.Map<List<InvestorWalletDTO>>(investorWalletList);
            return list;
        }

        //GET BY ID
        public async Task<InvestorWalletDTO> GetInvestorWalletById(Guid investorWalletId)
        {
            InvestorWalletDTO result;
            try
            {

                InvestorWallet dto = await _investorWalletRepository.GetInvestorWalletById(investorWalletId);
                result = _mapper.Map<InvestorWalletDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No InvestorWallet Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorWallet(InvestorWalletDTO investorWalletDTO, Guid investorWalletId)
        {
            int result;
            try
            {
                InvestorWallet dto = _mapper.Map<InvestorWallet>(investorWalletDTO);
                result = await _investorWalletRepository.UpdateInvestorWallet(dto, investorWalletId);
                if (result == 0)
                    throw new CreateObjectException("Can not update InvestorWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
