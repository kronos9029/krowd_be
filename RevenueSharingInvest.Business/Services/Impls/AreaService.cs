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
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IMapper _mapper;


        public AreaService(IAreaRepository areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateArea(AreaDTO areaDTO)
        {
            int result;
            try
            {
                Area dto = _mapper.Map<Area>(areaDTO);
                result = await _areaRepository.CreateArea(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Area Object!");
                return result;
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
                    throw new CreateObjectException("Can not delete Area Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<AreaDTO>> GetAllAreas()
        {
            List<Area> areaList = await _areaRepository.GetAllAreas();
            List<AreaDTO> list = _mapper.Map<List<AreaDTO>>(areaList);
            return list;
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
                    throw new CreateObjectException("No Area Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateArea(AreaDTO areaDTO, Guid areaId)
        {
            int result;
            try
            {
                Area dto = _mapper.Map<Area>(areaDTO);
                result = await _areaRepository.UpdateArea(dto, areaId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Area Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
