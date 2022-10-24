using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IBillService
    {
        //CREATE
        public Task<DailyReportDTO> BulkInsertBills(InsertBillDTO bills, string projectId, string date);

        //READ
        public Task<AllBillDTO> GetAllBills(int pageIndex, int pageSize, Guid dailyReportId, ThisUserObj currentUser);
        public Task<GetBillDTO> GetBillById(Guid id, ThisUserObj currentUser);

        //UPDATE

        //DELETE
    }
}
