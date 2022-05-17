using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IPeriodRevenueRepository
    {
        //CREATE
        public Task<int> CreatePeriodRevenue(PeriodRevenue periodRevenueDTO);

        //READ
        public Task<List<PeriodRevenue>> GetAllPeriodRevenues();
        public Task<PeriodRevenue> GetPeriodRevenueById(Guid periodRevenueId);

        //UPDATE
        public Task<int> UpdatePeriodRevenue(PeriodRevenue periodRevenueDTO, Guid periodRevenueId);

        //DELETE
        public Task<int> DeletePeriodRevenueById(Guid periodRevenueId);
    }
}
