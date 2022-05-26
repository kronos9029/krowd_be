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
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IMapper _mapper;

        public InvestorService(IInvestorRepository investorRepository, IInvestorWalletRepository investorWalletRepository, IMapper mapper)
        {
            _investorRepository = investorRepository;
            _investorWalletRepository = investorWalletRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreateInvestor(InvestorDTO investorDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                Investor dto = _mapper.Map<Investor>(investorDTO);
                newId.id = await _investorRepository.CreateInvestor(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Investor Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        //DELETE
        public async Task<int> DeleteInvestorById(Guid investorId)
        {
            int result;
            try
            {

                result = await _investorRepository.DeleteInvestorById(investorId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Investor Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestorDTO>> GetAllInvestors(int pageIndex, int pageSize)
        {
            List<Investor> investorList = await _investorRepository.GetAllInvestors(pageIndex, pageSize);
            List<InvestorDTO> list = _mapper.Map<List<InvestorDTO>>(investorList);
            return list;
        }

        //GET BY ID
        public async Task<InvestorDTO> GetInvestorById(Guid investorId)
        {
            InvestorDTO result;
            try
            {

                Investor dto = await _investorRepository.GetInvestorById(investorId);
                result = _mapper.Map<InvestorDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Investor Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestor(InvestorDTO investorDTO, Guid investorId)
        {
            int result;
            try
            {
                Investor dto = _mapper.Map<Investor>(investorDTO);
                result = await _investorRepository.UpdateInvestor(dto, investorId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Investor Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
