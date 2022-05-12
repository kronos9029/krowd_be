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
    public class BusinessFieldService :IBusinessFieldService
    {
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IMapper _mapper;

        //CREATE
        public async Task<int> CreateBusinessField(BusinessFieldDTO businessFieldDTO)
        {
            int result;
            try
            {
                BusinessField dto = _mapper.Map<BusinessField>(businessFieldDTO);
                result = await _businessFieldRepository.CreateBusinessField(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create BusinessField Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE    
        public async Task<int> DeleteBusinessFieldById(Guid businessFieldId)
        {
            int result;
            try
            {

                result = await _businessFieldRepository.DeleteBusinessFieldById(businessFieldId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete BusinessField Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<BusinessFieldDTO>> GetAllBusinessFields()
        {
            List<BusinessField> businessFieldList = await _businessFieldRepository.GetAllBusinessFields();
            List<BusinessFieldDTO> list = _mapper.Map<List<BusinessFieldDTO>>(businessFieldList);
            return list;
        }

        //UPDATE
        public async Task<int> UpdateBusinessField(BusinessFieldDTO businessFieldDTO, Guid businessFieldId)
        {
            int result;
            try
            {
                BusinessField dto = _mapper.Map<BusinessField>(businessFieldDTO);
                result = await _businessFieldRepository.UpdateBusinessField(dto, businessFieldId);
                if (result == 0)
                    throw new CreateObjectException("Can not update BusinessField Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
