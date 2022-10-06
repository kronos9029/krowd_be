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
    }
}
