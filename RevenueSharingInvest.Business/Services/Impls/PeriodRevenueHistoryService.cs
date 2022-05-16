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
    public class PeriodRevenueHistoryService : IPeriodRevenueHistoryService
    {
        private readonly IPeriodRevenueHistoryRepository _periodRevenueHistoryRepository;
        private readonly IMapper _mapper;


        public PeriodRevenueHistoryService(IPeriodRevenueHistoryRepository periodRevenueHistoryRepository, IMapper mapper)
        {
            _periodRevenueHistoryRepository = periodRevenueHistoryRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO)
        {
            int result;
            try
            {
                PeriodRevenueHistory dto = _mapper.Map<PeriodRevenueHistory>(periodRevenueHistoryDTO);
                result = await _periodRevenueHistoryRepository.CreatePeriodRevenueHistory(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create PeriodRevenueHistory Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeletePeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        {
            int result;
            try
            {

                result = await _periodRevenueHistoryRepository.DeletePeriodRevenueHistoryById(periodRevenueHistoryId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete PeriodRevenueHistory Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueHistoryDTO>> GetAllPeriodRevenueHistorys()
        {
            List<PeriodRevenueHistory> periodRevenueHistoryList = await _periodRevenueHistoryRepository.GetAllPeriodRevenueHistorys();
            List<PeriodRevenueHistoryDTO> list = _mapper.Map<List<PeriodRevenueHistoryDTO>>(periodRevenueHistoryList);
            return list;
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
                    throw new CreateObjectException("No PeriodRevenueHistory Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO, Guid periodRevenueHistoryId)
        {
            int result;
            try
            {
                PeriodRevenueHistory dto = _mapper.Map<PeriodRevenueHistory>(periodRevenueHistoryDTO);
                result = await _periodRevenueHistoryRepository.UpdatePeriodRevenueHistory(dto, periodRevenueHistoryId);
                if (result == 0)
                    throw new CreateObjectException("Can not update PeriodRevenueHistory Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
