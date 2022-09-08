using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
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
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public AreaService(IAreaRepository areaRepository, IValidationService validationService, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllAreaData()
        {
            int result;
            try
            {
                result = await _areaRepository.ClearAllAreaData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateArea(AreaDTO areaDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(areaDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (!await _validationService.CheckText(areaDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                if (areaDTO.createBy != null && areaDTO.createBy.Length >= 0)
                {
                    if (areaDTO.createBy.Equals("string"))
                        areaDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(areaDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (areaDTO.updateBy != null && areaDTO.updateBy.Length >= 0)
                {
                    if (areaDTO.updateBy.Equals("string"))
                        areaDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(areaDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                areaDTO.isDeleted = false;

                Area dto = _mapper.Map<Area>(areaDTO);
                newId.id = await _areaRepository.CreateArea(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Area Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteAreaById(Guid areaId)
        {
            int result;
            try
            {
                result = await _areaRepository.DeleteAreaById(areaId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Area Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<AreaDTO>> GetAllAreas(int pageIndex, int pageSize)
        {
            try
            {
                List<Area> areaList = await _areaRepository.GetAllAreas(pageIndex, pageSize);
                List<AreaDTO> list = _mapper.Map<List<AreaDTO>>(areaList);
                foreach (AreaDTO item in list)
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
        public async Task<AreaDTO> GetAreaById(Guid areaId)
        {
            AreaDTO result;
            try
            {

                Area dto = await _areaRepository.GetAreaById(areaId);
                result = _mapper.Map<AreaDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Area Object Found!");

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
        public async Task<int> UpdateArea(AreaDTO areaDTO, Guid areaId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(areaDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (!await _validationService.CheckText(areaDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                if (areaDTO.createBy != null && areaDTO.createBy.Length >= 0)
                {
                    if (areaDTO.createBy.Equals("string"))
                        areaDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(areaDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (areaDTO.updateBy != null && areaDTO.updateBy.Length >= 0)
                {
                    if (areaDTO.updateBy.Equals("string"))
                        areaDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(areaDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Area dto = _mapper.Map<Area>(areaDTO);
                result = await _areaRepository.UpdateArea(dto, areaId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Area Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
