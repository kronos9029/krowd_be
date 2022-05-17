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
    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        private readonly IMapper _mapper;


        public StageService(IStageRepository stageRepository, IMapper mapper)
        {
            _stageRepository = stageRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateStage(StageDTO stageDTO)
        {
            int result;
            try
            {
                Stage dto = _mapper.Map<Stage>(stageDTO);
                result = await _stageRepository.CreateStage(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Stage Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteStageById(Guid stageId)
        {
            int result;
            try
            {

                result = await _stageRepository.DeleteStageById(stageId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Stage Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<StageDTO>> GetAllStages()
        {
            List<Stage> stageList = await _stageRepository.GetAllStages();
            List<StageDTO> list = _mapper.Map<List<StageDTO>>(stageList);
            return list;
        }

        //GET BY ID
        public async Task<StageDTO> GetStageById(Guid stageId)
        {
            StageDTO result;
            try
            {

                Stage dto = await _stageRepository.GetStageById(stageId);
                result = _mapper.Map<StageDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Stage Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateStage(StageDTO stageDTO, Guid stageId)
        {
            int result;
            try
            {
                Stage dto = _mapper.Map<Stage>(stageDTO);
                result = await _stageRepository.UpdateStage(dto, stageId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Stage Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
