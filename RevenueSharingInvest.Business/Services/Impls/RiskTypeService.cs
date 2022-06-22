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
    public class RiskTypeService : IRiskTypeService
    {
        private readonly IRiskTypeRepository _riskTypeRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public RiskTypeService(IRiskTypeRepository riskTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _riskTypeRepository = riskTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllRiskTypeData()
        {
            int result;
            try
            {
                result = await _riskTypeRepository.ClearAllRiskTypeData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateRiskType(RiskTypeDTO riskTypeDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(riskTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (riskTypeDTO.description != null && (riskTypeDTO.description.Equals("string") || riskTypeDTO.description.Length == 0))
                    riskTypeDTO.description = null;

                if (riskTypeDTO.createBy != null && riskTypeDTO.createBy.Length >= 0)
                {
                    if (riskTypeDTO.createBy.Equals("string"))
                        riskTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (riskTypeDTO.updateBy != null && riskTypeDTO.updateBy.Length >= 0)
                {
                    if (riskTypeDTO.updateBy.Equals("string"))
                        riskTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                riskTypeDTO.isDeleted = false;

                RiskType dto = _mapper.Map<RiskType>(riskTypeDTO);
                newId.id = await _riskTypeRepository.CreateRiskType(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create RiskType Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete RiskType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RiskTypeDTO>> GetAllRiskTypes(int pageIndex, int pageSize)
        {
            try
            {
                List<RiskType> riskTypeList = await _riskTypeRepository.GetAllRiskTypes(pageIndex, pageSize);
                List<RiskTypeDTO> list = _mapper.Map<List<RiskTypeDTO>>(riskTypeList);

                foreach (RiskTypeDTO item in list)
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
        public async Task<RiskTypeDTO> GetRiskTypeById(Guid riskTypeId)
        {
            RiskTypeDTO result;
            try
            {

                RiskType dto = await _riskTypeRepository.GetRiskTypeById(riskTypeId);
                result = _mapper.Map<RiskTypeDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No RiskType Object Found!");

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
        public async Task<int> UpdateRiskType(RiskTypeDTO riskTypeDTO, Guid riskTypeId)
        {
            int result;
            try
            {

                if (!await _validationService.CheckText(riskTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (riskTypeDTO.description != null && (riskTypeDTO.description.Equals("string") || riskTypeDTO.description.Length == 0))
                    riskTypeDTO.description = null;

                if (riskTypeDTO.createBy != null && riskTypeDTO.createBy.Length >= 0)
                {
                    if (riskTypeDTO.createBy.Equals("string"))
                        riskTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (riskTypeDTO.updateBy != null && riskTypeDTO.updateBy.Length >= 0)
                {
                    if (riskTypeDTO.updateBy.Equals("string"))
                        riskTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                RiskType dto = _mapper.Map<RiskType>(riskTypeDTO);
                result = await _riskTypeRepository.UpdateRiskType(dto, riskTypeId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update RiskType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
