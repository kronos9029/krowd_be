using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPeriodRevenueService
    {
        //CREATE
        //public Task<IdDTO> CreatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO);

        //READ
        public Task<List<GetPeriodRevenueDTO>> GetAllPeriodRevenues(int pageIndex, int pageSize);
        public Task<GetPeriodRevenueDTO> GetPeriodRevenueById(Guid periodRevenueId);

        //UPDATE
        //public Task<int> UpdatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO, Guid periodRevenueId);

        //DELETE
        //public Task<int> DeletePeriodRevenueById(Guid periodRevenueId);
        //public Task<int> ClearAllPeriodRevenueData();
    }
}
