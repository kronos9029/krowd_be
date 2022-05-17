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
    public class InvestorTypeService : IInvestorTypeService
    {
        private readonly IInvestorTypeRepository _investorTypeRepository;
        private readonly IMapper _mapper;


        public InvestorTypeService(IInvestorTypeRepository investorTypeRepository, IMapper mapper)
        {
            _investorTypeRepository = investorTypeRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateInvestorType(InvestorTypeDTO investorTypeDTO)
        {
            int result;
            try
            {
                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                result = await _investorTypeRepository.CreateInvestorType(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteInvestorTypeById(Guid investorTypeId)
        {
            int result;
            try
            {

                result = await _investorTypeRepository.DeleteInvestorTypeById(investorTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<InvestorTypeDTO>> GetAllInvestorTypes()
        {
            List<InvestorType> investorTypeList = await _investorTypeRepository.GetAllInvestorTypes();
            List<InvestorTypeDTO> list = _mapper.Map<List<InvestorTypeDTO>>(investorTypeList);
            return list;
        }

        //GET BY ID
        public async Task<InvestorTypeDTO> GetInvestorTypeById(Guid investorTypeId)
        {
            InvestorTypeDTO result;
            try
            {

                InvestorType dto = await _investorTypeRepository.GetInvestorTypeById(investorTypeId);
                result = _mapper.Map<InvestorTypeDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No InvestorType Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorType(InvestorTypeDTO investorTypeDTO, Guid investorTypeId)
        {
            int result;
            try
            {
                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                result = await _investorTypeRepository.UpdateInvestorType(dto, investorTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not update InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
