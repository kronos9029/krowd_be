using AutoMapper;
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
    public class PeriodRevenueHistoryService : IPeriodRevenueHistoryService
    {
        private readonly IPeriodRevenueHistoryRepository _periodRevenueHistoryRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PeriodRevenueHistoryService(IPeriodRevenueHistoryRepository periodRevenueHistoryRepository, IValidationService validationService, IMapper mapper)
        {
            _periodRevenueHistoryRepository = periodRevenueHistoryRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(periodRevenueHistoryDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (periodRevenueHistoryDTO.periodRevenueId == null || !await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.periodRevenueId))
                    throw new InvalidFieldException("Invalid periodRevenueId!!!");

                if (!await _validationService.CheckExistenceId("PeriodRevenue", Guid.Parse(periodRevenueHistoryDTO.periodRevenueId)))
                    throw new NotFoundException("This periodRevenueId is not existed!!!");

                if (periodRevenueHistoryDTO.description != null && (periodRevenueHistoryDTO.description.Equals("string") || periodRevenueHistoryDTO.description.Length == 0))
                    periodRevenueHistoryDTO.description = null;

                if (!await _validationService.CheckText(periodRevenueHistoryDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (periodRevenueHistoryDTO.createBy != null && periodRevenueHistoryDTO.createBy.Length >= 0)
                {
                    if (periodRevenueHistoryDTO.createBy.Equals("string"))
                        periodRevenueHistoryDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (periodRevenueHistoryDTO.updateBy != null && periodRevenueHistoryDTO.updateBy.Length >= 0)
                {
                    if (periodRevenueHistoryDTO.updateBy.Equals("string"))
                        periodRevenueHistoryDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                periodRevenueHistoryDTO.isDeleted = false;

                PeriodRevenueHistory dto = _mapper.Map<PeriodRevenueHistory>(periodRevenueHistoryDTO);
                newId.id = await _periodRevenueHistoryRepository.CreatePeriodRevenueHistory(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create PeriodRevenueHistory Object!");
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        //public async Task<int> DeletePeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        //{
        //    int result;
        //    try
        //    {
        //        result = await _periodRevenueHistoryRepository.DeletePeriodRevenueHistoryById(periodRevenueHistoryId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete PeriodRevenueHistory Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //GET ALL
        public async Task<List<PeriodRevenueHistoryDTO>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize)
        {
            try
            {
                List<PeriodRevenueHistory> periodRevenueHistoryList = await _periodRevenueHistoryRepository.GetAllPeriodRevenueHistories(pageIndex, pageSize);
                List<PeriodRevenueHistoryDTO> list = _mapper.Map<List<PeriodRevenueHistoryDTO>>(periodRevenueHistoryList);

                foreach (PeriodRevenueHistoryDTO item in list)
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
        public async Task<PeriodRevenueHistoryDTO> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        {
            PeriodRevenueHistoryDTO result;
            try
            {
                PeriodRevenueHistory dto = await _periodRevenueHistoryRepository.GetPeriodRevenueHistoryById(periodRevenueHistoryId);
                result = _mapper.Map<PeriodRevenueHistoryDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No PeriodRevenueHistory Object Found!");

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
        public async Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO, Guid periodRevenueHistoryId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(periodRevenueHistoryDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (periodRevenueHistoryDTO.periodRevenueId == null || !await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.periodRevenueId))
                    throw new InvalidFieldException("Invalid periodRevenueId!!!");

                if (!await _validationService.CheckExistenceId("PeriodRevenue", Guid.Parse(periodRevenueHistoryDTO.periodRevenueId)))
                    throw new NotFoundException("This periodRevenueId is not existed!!!");

                if (periodRevenueHistoryDTO.description != null && (periodRevenueHistoryDTO.description.Equals("string") || periodRevenueHistoryDTO.description.Length == 0))
                    periodRevenueHistoryDTO.description = null;

                if (!await _validationService.CheckText(periodRevenueHistoryDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (periodRevenueHistoryDTO.createBy != null && periodRevenueHistoryDTO.createBy.Length >= 0)
                {
                    if (periodRevenueHistoryDTO.createBy.Equals("string"))
                        periodRevenueHistoryDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (periodRevenueHistoryDTO.updateBy != null && periodRevenueHistoryDTO.updateBy.Length >= 0)
                {
                    if (periodRevenueHistoryDTO.updateBy.Equals("string"))
                        periodRevenueHistoryDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueHistoryDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                PeriodRevenueHistory dto = _mapper.Map<PeriodRevenueHistory>(periodRevenueHistoryDTO);
                result = await _periodRevenueHistoryRepository.UpdatePeriodRevenueHistory(dto, periodRevenueHistoryId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update PeriodRevenueHistory Object!");
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
