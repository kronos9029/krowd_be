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
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IMapper _mapper;


        public FieldService(IFieldRepository fieldRepository, IMapper mapper)
        {
            _fieldRepository = fieldRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateField(FieldDTO fieldDTO)
        {
            int result;
            try
            {
                Field dto = _mapper.Map<Field>(fieldDTO);
                result = await _fieldRepository.CreateField(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Field Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteFieldById(Guid fieldId)
        {
            int result;
            try
            {

                result = await _fieldRepository.DeleteFieldById(fieldId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Field Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<FieldDTO>> GetAllFields()
        {
            List<Field> areaList = await _fieldRepository.GetAllFields();
            List<FieldDTO> list = _mapper.Map<List<FieldDTO>>(areaList);
            return list;
        }

        //GET BY ID
        public async Task<FieldDTO> GetFieldById(Guid fieldId)
        {
            FieldDTO result;
            try
            {

                Field dto = await _fieldRepository.GetFieldById(fieldId);
                result = _mapper.Map<FieldDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Field Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateField(FieldDTO fieldDTO, Guid fieldId)
        {
            int result;
            try
            {
                Field dto = _mapper.Map<Field>(fieldDTO);
                result = await _fieldRepository.UpdateField(dto, fieldId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Field Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
