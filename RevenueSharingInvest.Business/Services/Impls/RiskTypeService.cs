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
    public class RiskTypeService : IRiskTypeService
    {
        private readonly IRiskTypeRepository _riskTypeRepository;
        private readonly IMapper _mapper;


        public RiskTypeService(IRiskTypeRepository riskTypeRepository, IMapper mapper)
        {
            _riskTypeRepository = riskTypeRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateRiskType(RiskTypeDTO riskTypeDTO)
        {
            int result;
            try
            {
                RiskType dto = _mapper.Map<RiskType>(riskTypeDTO);
                result = await _riskTypeRepository.CreateRiskType(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create RiskType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteRiskTypeById(Guid riskTypeId)
        {
            int result;
            try
            {

                result = await _riskTypeRepository.DeleteRiskTypeById(riskTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete RiskType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<RiskTypeDTO>> GetAllRiskTypes()
        {
            List<RiskType> riskTypeList = await _riskTypeRepository.GetAllRiskTypes();
            List<RiskTypeDTO> list = _mapper.Map<List<RiskTypeDTO>>(riskTypeList);
            return list;
        }

        //GET BY ID
        public async Task<RiskTypeDTO> GetRiskTypeById(Guid riskTypeId)
        {
            RiskTypeDTO result;
            try
            {

                RiskType dto = await _riskTypeRepository.GetRiskTypeById(riskTypeId);
                result = _mapper.Map<RiskTypeDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No RiskType Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateRiskType(RiskTypeDTO riskTypeDTO, Guid riskTypeId)
        {
            int result;
            try
            {
                RiskType dto = _mapper.Map<RiskType>(riskTypeDTO);
                result = await _riskTypeRepository.UpdateRiskType(dto, riskTypeId);
                if (result == 0)
                    throw new CreateObjectException("Can not update RiskType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
