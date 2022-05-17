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
        public Task<int> CreatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO);

        //READ
        public Task<List<PeriodRevenueDTO>> GetAllPeriodRevenues();
        public Task<PeriodRevenueDTO> GetPeriodRevenueById(Guid periodRevenueId);

        //UPDATE
        public Task<int> UpdatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO, Guid periodRevenueId);

        //DELETE
        public Task<int> DeletePeriodRevenueById(Guid periodRevenueId);
    }
}
