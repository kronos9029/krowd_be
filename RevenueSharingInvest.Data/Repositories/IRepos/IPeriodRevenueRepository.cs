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
        public Task<string> CreatePeriodRevenue(PeriodRevenue periodRevenueDTO);
        public Task<string> CreateRepaymentPeriodRevenue(PeriodRevenue periodRevenueDTO);

        //READ
        public Task<List<PeriodRevenue>> GetAllPeriodRevenues(int pageIndex, int pageSize);
        public Task<PeriodRevenue> GetPeriodRevenueById(Guid periodRevenueId);
        public Task<PeriodRevenue> GetPeriodRevenueByStageId(Guid stageId);
        public Task<int> CountNotPaidEnoughPeriodRevenue(Guid projectId);
        public Task<double> SumSharedAmount(Guid projectId);

        //UPDATE
        public Task<int> UpdatePeriodRevenue(PeriodRevenue periodRevenueDTO);
        public Task<int> UpdatePeriodRevenueByStageId(PeriodRevenue periodRevenueDTO, Guid stageId);
        public Task<int> UpdatePeriodRevenueByPaidAmount(PeriodRevenue periodRevenueDTO);

        //DELETE
        public Task<int> DeletePeriodRevenueByStageId(Guid stageId);
        public Task<int> DeletePeriodRevenueByProjectId(Guid projectId);
    }
}
