using AutoMapper;
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
    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public StageService(IStageRepository stageRepository, IValidationService validationService, IMapper mapper)
        {
            _stageRepository = stageRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllStageData()
        {
            int result;
            try
            {
                result = await _stageRepository.ClearAllStageData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateStage(StageDTO stageDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(stageDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (stageDTO.projectId == null || !await _validationService.CheckUUIDFormat(stageDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(stageDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (stageDTO.description != null && (stageDTO.description.Equals("string") || stageDTO.description.Length == 0))
                    stageDTO.description = null;

                if (stageDTO.percents <= 0)
                    throw new InvalidFieldException("percents must be greater than 0!!!");

                if (stageDTO.openMonth <= 0)
                    throw new InvalidFieldException("openMonth must be greater than 0!!!");

                if (stageDTO.closeMonth <= 0)
                    throw new InvalidFieldException("closeMonth must be greater than 0!!!");

                if (!await _validationService.CheckText(stageDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (stageDTO.createBy != null && stageDTO.createBy.Length >= 0)
                {
                    if (stageDTO.createBy.Equals("string"))
                        stageDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(stageDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (stageDTO.updateBy != null && stageDTO.updateBy.Length >= 0)
                {
                    if (stageDTO.updateBy.Equals("string"))
                        stageDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(stageDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                stageDTO.isDeleted = false;

                Stage dto = _mapper.Map<Stage>(stageDTO);
                newId.id = await _stageRepository.CreateStage(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Stage Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete Stage Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<StageDTO>> GetAllStages(int pageIndex, int pageSize)
        {
            try
            {
                List<Stage> stageList = await _stageRepository.GetAllStages(pageIndex, pageSize);
                List<StageDTO> list = _mapper.Map<List<StageDTO>>(stageList);

                foreach (StageDTO item in list)
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
        public async Task<StageDTO> GetStageById(Guid stageId)
        {
            StageDTO result;
            try
            {

                Stage dto = await _stageRepository.GetStageById(stageId);
                result = _mapper.Map<StageDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Stage Object Found!");

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
        public async Task<int> UpdateStage(StageDTO stageDTO, Guid stageId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(stageDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (stageDTO.projectId == null || !await _validationService.CheckUUIDFormat(stageDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(stageDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (stageDTO.description != null && (stageDTO.description.Equals("string") || stageDTO.description.Length == 0))
                    stageDTO.description = null;

                if (stageDTO.percents <= 0)
                    throw new InvalidFieldException("percents must be greater than 0!!!");

                if (stageDTO.openMonth <= 0)
                    throw new InvalidFieldException("openMonth must be greater than 0!!!");

                if (stageDTO.closeMonth <= 0)
                    throw new InvalidFieldException("closeMonth must be greater than 0!!!");

                if (!await _validationService.CheckText(stageDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (stageDTO.createBy != null && stageDTO.createBy.Length >= 0)
                {
                    if (stageDTO.createBy.Equals("string"))
                        stageDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(stageDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (stageDTO.updateBy != null && stageDTO.updateBy.Length >= 0)
                {
                    if (stageDTO.updateBy.Equals("string"))
                        stageDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(stageDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Stage dto = _mapper.Map<Stage>(stageDTO);
                result = await _stageRepository.UpdateStage(dto, stageId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Stage Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
