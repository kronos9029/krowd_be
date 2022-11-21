using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IDailyReportRepository
    {
        //CREATE
        public Task<int> CreateDailyReports(DailyReport dailyReport, int numOfReport);

        //READ
        public Task<List<DailyReport>> GetAllDailyReports(int pageIndex, int pageSize, string projectId, string stageId, string roleId);
        public Task<List<DailyReport>> GetAllDailyReportsByStageId(Guid stageId);
        public Task<int> CountAllDailyReports(string projectId, string stageId, string roleId);
        public Task<DailyReport> GetDailyReportById(Guid id);
        public Task<DailyReport> GetDailyReportByProjectIdAndDate(Guid projectId, string date);
        public Task<int> CountNotReportedDailyReportsByStageId(Guid stageId);

        //UPDATE
        public Task<int> UpdateDailyReport(DailyReport dailyReport);

        //DELETE
        public Task<int> DeleteDailyReportByProjectId(Guid projectId);
    }
}
