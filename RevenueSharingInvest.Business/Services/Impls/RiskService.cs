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
    public class RiskService : IRiskService
    {
        private readonly IRiskRepository _riskRepository;
        private readonly IMapper _mapper;


        public RiskService(IRiskRepository riskRepository, IMapper mapper)
        {
            _riskRepository = riskRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateRisk(RiskDTO riskDTO)
        {
            int result;
            try
            {
                Risk dto = _mapper.Map<Risk>(riskDTO);
                result = await _riskRepository.CreateRisk(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Risk Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteRiskById(Guid riskId)
        {
            int result;
            try
            {

                result = await _riskRepository.DeleteRiskById(riskId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Risk Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<RiskDTO>> GetAllRisks()
        {
            List<Risk> riskList = await _riskRepository.GetAllRisks();
            List<RiskDTO> list = _mapper.Map<List<RiskDTO>>(riskList);
            return list;
        }

        //GET BY ID
        public async Task<RiskDTO> GetRiskById(Guid riskId)
        {
            RiskDTO result;
            try
            {

                Risk dto = await _riskRepository.GetRiskById(riskId);
                result = _mapper.Map<RiskDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Risk Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateRisk(RiskDTO riskDTO, Guid riskId)
        {
            int result;
            try
            {
                Risk dto = _mapper.Map<Risk>(riskDTO);
                result = await _riskRepository.UpdateRisk(dto, riskId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Risk Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
