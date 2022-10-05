using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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

                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                newId.id = await _investorTypeRepository.CreateInvestorType(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create InvestorType Object!");
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<GetInvestorTypeDTO>> GetAllInvestorTypes(int pageIndex, int pageSize)
        {
            try
            {
                List<InvestorType> investorTypeList = await _investorTypeRepository.GetAllInvestorTypes(pageIndex, pageSize);
                List<GetInvestorTypeDTO> list = _mapper.Map<List<GetInvestorTypeDTO>>(investorTypeList);

                foreach (GetInvestorTypeDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetInvestorTypeDTO> GetInvestorTypeById(Guid investorTypeId)
        {
            GetInvestorTypeDTO result;
            try
            {

                InvestorType dto = await _investorTypeRepository.GetInvestorTypeById(investorTypeId);
                result = _mapper.Map<GetInvestorTypeDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No InvestorType Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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

                InvestorType dto = _mapper.Map<InvestorType>(investorTypeDTO);
                result = await _investorTypeRepository.UpdateInvestorType(dto, investorTypeId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update InvestorType Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
