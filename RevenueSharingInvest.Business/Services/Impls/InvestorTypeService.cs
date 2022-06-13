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
    public class InvestorTypeService : IInvestorTypeService
    {
        private readonly IInvestorTypeRepository _investorTypeRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public InvestorTypeService(IInvestorTypeRepository investorTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _investorTypeRepository = investorTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<int> ClearAllInvestorTypeData()
        {
            int result;
            try
            {
                result = await _investorTypeRepository.ClearAllInvestorTypeData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateInvestorType(InvestorTypeDTO investorTypeDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(investorTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (investorTypeDTO.description != null && (investorTypeDTO.description.Equals("string") || investorTypeDTO.description.Length == 0))
                    investorTypeDTO.description = null;

                if (investorTypeDTO.createBy != null && investorTypeDTO.createBy.Length >= 0)
                {
                    if (investorTypeDTO.createBy.Equals("string"))
                        investorTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investorTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (investorTypeDTO.updateBy != null && investorTypeDTO.updateBy.Length >= 0)
                {
                    if (investorTypeDTO.updateBy.Equals("string"))
                        investorTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investorTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                investorTypeDTO.isDeleted = false;

                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                newId.id = await _investorTypeRepository.CreateInvestorType(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create InvestorType Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestorTypeDTO>> GetAllInvestorTypes(int pageIndex, int pageSize)
        {
            try
            {
                List<InvestorType> investorTypeList = await _investorTypeRepository.GetAllInvestorTypes(pageIndex, pageSize);
                List<InvestorTypeDTO> list = _mapper.Map<List<InvestorTypeDTO>>(investorTypeList);
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No InvestorType Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorType(InvestorTypeDTO investorTypeDTO, Guid investorTypeId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(investorTypeDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (investorTypeDTO.description != null && (investorTypeDTO.description.Equals("string") || investorTypeDTO.description.Length == 0))
                    investorTypeDTO.description = null;

                if (investorTypeDTO.createBy != null && investorTypeDTO.createBy.Length >= 0)
                {
                    if (investorTypeDTO.createBy.Equals("string"))
                        investorTypeDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investorTypeDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (investorTypeDTO.updateBy != null && investorTypeDTO.updateBy.Length >= 0)
                {
                    if (investorTypeDTO.updateBy.Equals("string"))
                        investorTypeDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investorTypeDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                result = await _investorTypeRepository.UpdateInvestorType(dto, investorTypeId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
