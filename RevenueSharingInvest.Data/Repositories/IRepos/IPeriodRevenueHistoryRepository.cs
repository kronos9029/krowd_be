using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IPeriodRevenueHistoryRepository
    {
        //CREATE
        public Task<int> CreatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO);

        //READ
        public Task<List<PeriodRevenueHistory>> GetAllPeriodRevenueHistories();
        public Task<PeriodRevenueHistory> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId);

        //UPDATE
        public Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO, Guid periodRevenueHistoryId);

        //DELETE
        public Task<int> DeletePeriodRevenueHistoryById(Guid periodRevenueHistoryId);
    }
}
