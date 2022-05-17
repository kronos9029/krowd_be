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
    public class InvestmentService : IInvestmentService
    {
        //private readonly IInvestorRepository _investorRepository;
        //private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IMapper _mapper;

        public InvestmentService(IInvestmentRepository investmentRepository, IMapper mapper)
        {
            //_investorRepository = investorRepository;
            //_investorWalletRepository = investorWalletRepository;
            _investmentRepository = investmentRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateInvestment(InvestmentDTO investmentDTO)
        {
            int result;
            try
            {
                Investment dto = _mapper.Map<Investment>(investmentDTO);
                result = await _investmentRepository.CreateInvestment(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Investment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteInvestmentById(Guid investmentId)
        {
            int result;
            try
            {

                result = await _investmentRepository.DeleteInvestmentById(investmentId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Investment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<InvestmentDTO>> GetAllInvestments()
        {
            List<Investment> investmentList = await _investmentRepository.GetAllInvestments();
            List<InvestmentDTO> list = _mapper.Map<List<InvestmentDTO>>(investmentList);
            return list;
        }

        //GET BY ID
        public async Task<InvestmentDTO> GetInvestmentById(Guid investmentId)
        {
            InvestmentDTO result;
            try
            {

                Investment dto = await _investmentRepository.GetInvestmentById(investmentId);
                result = _mapper.Map<InvestmentDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Investment Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId)
        {
            int result;
            try
            {
                Investment dto = _mapper.Map<Investment>(investmentId);
                result = await _investmentRepository.UpdateInvestment(dto, investmentId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Investment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
