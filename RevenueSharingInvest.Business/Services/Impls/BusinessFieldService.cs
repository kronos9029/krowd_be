using AutoMapper;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
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
    public class BusinessFieldService :IBusinessFieldService
    {
        private readonly AppSettings _appSettings;
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public BusinessFieldService(IOptions<AppSettings> appSettings, IBusinessFieldRepository businessFieldRepository, IValidationService validationService, IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _businessFieldRepository = businessFieldRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllBusinessFieldData()
        {
            int result;
            try
            {
                result = await _businessFieldRepository.ClearAllBusinessFieldData();
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //CREATE
        //public async Task<int> CreateBusinessField(BusinessFieldDTO businessFieldDTO)
        //{
        //    int result;
        //    try
        //    {
        //        if(businessFieldDTO.businessId == null || !await _validationService.CheckUUIDFormat(businessFieldDTO.businessId))
        //            throw new InvalidFieldException("Invalid businessId!!!");

        //        if(!await _validationService.CheckExistenceId("Business", Guid.Parse(businessFieldDTO.businessId)))
        //            throw new NotFoundException("This businessId is not existed!!!");

        //        if (businessFieldDTO.fieldId == null || !await _validationService.CheckUUIDFormat(businessFieldDTO.fieldId))
        //            throw new InvalidFieldException("Invalid fieldId!!!");

        //        if (!await _validationService.CheckExistenceId("Field", Guid.Parse(businessFieldDTO.fieldId)))
        //            throw new NotFoundException("This fieldId is not existed!!!");

        //        if (businessFieldDTO.createBy != null && businessFieldDTO.createBy.Length >= 0)
        //        {
        //            if (businessFieldDTO.createBy.Equals("string"))
        //                businessFieldDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(businessFieldDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (businessFieldDTO.updateBy != null && businessFieldDTO.updateBy.Length >= 0)
        //        {
        //            if (businessFieldDTO.updateBy.Equals("string"))
        //                businessFieldDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(businessFieldDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        businessFieldDTO.isDeleted = false;

        //        BusinessField dto = _mapper.Map<BusinessField>(businessFieldDTO);
        //        result = await _businessFieldRepository.CreateBusinessField(dto);
        //        if (result == 0)
        //            throw new CreateObjectException("Can not create BusinessField Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //DELETE    
        public async Task<int> DeleteBusinessFieldById(Guid businessId, Guid fieldId)
        {
            int result;
            try
            {

                result = await _businessFieldRepository.DeleteBusinessFieldById(businessId, fieldId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete BusinessField Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<BusinessFieldDTO>> GetAllBusinessFields(int pageIndex, int pageSize)
        {
            try
            {
                List<BusinessField> businessFieldList = await _businessFieldRepository.GetAllBusinessFields(pageIndex, pageSize);
                List<BusinessFieldDTO> list = _mapper.Map<List<BusinessFieldDTO>>(businessFieldList);

                foreach (BusinessFieldDTO item in list)
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
        public async Task<BusinessFieldDTO> GetBusinessFieldById(Guid businessId, Guid fieldId)
        {
            BusinessFieldDTO result;
            try
            {

                BusinessField dto = await _businessFieldRepository.GetBusinessFieldById(businessId, fieldId);
                result = _mapper.Map<BusinessFieldDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No BusinessField Object Found!");

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
        public async Task<int> UpdateBusinessField(BusinessFieldDTO businessFieldDTO, Guid businessId, Guid fieldId)
        {
            int result;
            try
            {
                if (businessFieldDTO.businessId == null || !await _validationService.CheckUUIDFormat(businessFieldDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId!!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessFieldDTO.businessId)))
                    throw new NotFoundException("This businessId is not existed!!!");

                if (businessFieldDTO.fieldId == null || !await _validationService.CheckUUIDFormat(businessFieldDTO.fieldId))
                    throw new InvalidFieldException("Invalid fieldId!!!");

                if (!await _validationService.CheckExistenceId("Field", Guid.Parse(businessFieldDTO.fieldId)))
                    throw new NotFoundException("This fieldId is not existed!!!");

                if (businessFieldDTO.createBy != null && businessFieldDTO.createBy.Length >= 0)
                {
                    if (businessFieldDTO.createBy.Equals("string"))
                        businessFieldDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessFieldDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (businessFieldDTO.updateBy != null && businessFieldDTO.updateBy.Length >= 0)
                {
                    if (businessFieldDTO.updateBy.Equals("string"))
                        businessFieldDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessFieldDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                BusinessField dto = _mapper.Map<BusinessField>(businessFieldDTO);
                result = await _businessFieldRepository.UpdateBusinessField(dto, businessId, fieldId);
                if (result == 0)
                    throw new CreateObjectException("Can not update BusinessField Object!");
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
