using AutoMapper;
using RevenueSharingInvest.API;
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

        //CREATE
        public async Task<IdDTO> CreateArea(CreateUpdateAreaDTO areaDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(areaDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (!await _validationService.CheckText(areaDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                Area entity = _mapper.Map<Area>(areaDTO);
                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                newId.id = await _areaRepository.CreateArea(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Area Object!");

                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteAreaById(Guid areaId, ThisUserObj currentUser)
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<GetAreaDTO>> GetAllAreas(int pageIndex, int pageSize, ThisUserObj currentUser)
        {
            try
            {
                List<Area> areaList = await _areaRepository.GetAllAreas(pageIndex, pageSize);
                List<GetAreaDTO> list = _mapper.Map<List<GetAreaDTO>>(areaList);
                foreach (GetAreaDTO item in list)
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
        public async Task<GetAreaDTO> GetAreaById(Guid areaId, ThisUserObj currentUser)
        {
            GetAreaDTO result;
            try
            {

                Area dto = await _areaRepository.GetAreaById(areaId);
                result = _mapper.Map<GetAreaDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Area Object Found!");

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
        public async Task<int> UpdateArea(CreateUpdateAreaDTO areaDTO, Guid areaId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                if (areaDTO.city != null && !await _validationService.CheckText(areaDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (areaDTO.district != null && !await _validationService.CheckText(areaDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                Area entity = _mapper.Map<Area>(areaDTO);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _areaRepository.UpdateArea(entity, areaId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Area Object!");

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
