﻿using AutoMapper;
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
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public FieldService(IFieldRepository fieldRepository, IValidationService validationService, IMapper mapper)
        {
            _fieldRepository = fieldRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<int> ClearAllFieldData()
        {
            int result;
            try
            {
                result = await _fieldRepository.ClearAllFieldData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateField(FieldDTO fieldDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(fieldDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (fieldDTO.description != null && (fieldDTO.description.Equals("string") || fieldDTO.description.Length == 0))
                    fieldDTO.description = null;

                if (fieldDTO.createBy != null && fieldDTO.createBy.Length >= 0)
                {
                    if (fieldDTO.createBy.Equals("string"))
                        fieldDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(fieldDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (fieldDTO.updateBy != null && fieldDTO.updateBy.Length >= 0)
                {
                    if (fieldDTO.updateBy.Equals("string"))
                        fieldDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(fieldDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                fieldDTO.isDeleted = false;

                Field dto = _mapper.Map<Field>(fieldDTO);
                newId.id = await _fieldRepository.CreateField(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Field Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete Field Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<FieldDTO>> GetAllFields(int pageIndex, int pageSize)
        {
            try
            {
                List<Field> areaList = await _fieldRepository.GetAllFields(pageIndex, pageSize);
                List<FieldDTO> list = _mapper.Map<List<FieldDTO>>(areaList);
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No Field Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateField(FieldDTO fieldDTO, Guid fieldId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(fieldDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (fieldDTO.description != null && (fieldDTO.description.Equals("string") || fieldDTO.description.Length == 0))
                    fieldDTO.description = null;

                if (fieldDTO.createBy != null && fieldDTO.createBy.Length >= 0)
                {
                    if (fieldDTO.createBy.Equals("string"))
                        fieldDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(fieldDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (fieldDTO.updateBy != null && fieldDTO.updateBy.Length >= 0)
                {
                    if (fieldDTO.updateBy.Equals("string"))
                        fieldDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(fieldDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Field dto = _mapper.Map<Field>(fieldDTO);
                result = await _fieldRepository.UpdateField(dto, fieldId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Field Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}