using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IBillRepository
    {
        //CREATE
        public Task<int> BulkInsertInvoice(InsertBillDTO bills);

        //READ
        public Task<List<Bill>> GetAllBills(int pageIndex, int pageSize, Guid dailyReportId);
        public Task<int> CountAllBills(Guid dailyReportId);
        public Task<Bill> GetBillById(Guid id);

        //UPDATE

        //DELETE
        public Task<int> DeleteBillByProjectId(Guid projectId);
        public Task<int> DeleteBillsByDailyReportId(Guid dailyReportId);
    }
}
