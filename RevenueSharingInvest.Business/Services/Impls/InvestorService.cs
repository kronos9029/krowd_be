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

        public async Task<InvestorDTO> GetInvestorById(string ID)
        {
            try
            {
                Investor result = await _investorRepository.GetInvestorByID(ID);
                if (result == null)
                    throw new NotFoundException("Investor With Id " + ID + " Not Found!!");
                InvestorDTO investorDTO = _mapper.Map<InvestorDTO>(result);
                investorDTO.balance = await _investorWalletRepository.GetInvestorTotalBalance(investorDTO.ID);
                return investorDTO;
            }
            catch (Exception e)
            {
                throw new Exception("Error at InvestorService: " + e.Message);
            }
        }

        public async Task<List<ProjectMemberDTO>> GetProjectMembers(string projectID)
        {
            try
            {
                ProjectMemberDTO ceo = await _investorRepository.GetInvestorByID(ID);
                if (result == null)
                    throw new NotFoundException("Investor With Id " + ID + " Not Found!!");
                InvestorDTO investorDTO = _mapper.Map<InvestorDTO>(result);
                investorDTO.balance = await _investorWalletRepository.GetInvestorTotalBalance(investorDTO.ID);
                return investorDTO;
            }
            catch (Exception e)
            {
                throw new Exception("Error at InvestorService: " + e.Message);
            }
        }
    }
}
