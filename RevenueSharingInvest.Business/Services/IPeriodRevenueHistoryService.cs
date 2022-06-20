using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPeriodRevenueHistoryService
    {
        //CREATE
        public Task<IdDTO> CreatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO);

        //READ
        public Task<List<PeriodRevenueHistoryDTO>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize);
        public Task<PeriodRevenueHistoryDTO> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId);

        //UPDATE
        public Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO, Guid periodRevenueHistoryId);

        //DELETE
        public Task<int> DeletePeriodRevenueHistoryById(Guid periodRevenueHistoryId);
        public Task<int> ClearAllPeriodRevenueHistoryData();
    }
}
