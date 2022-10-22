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
    public interface IDailyReportService
    {
        //CREATE

        //READ
        public Task<AllDailyReportDTO> GetAllDailyReports(int pageIndex, int pageSize, Guid projectId, Guid? stageId, ThisUserObj currentUser);
        public Task<DailyReportDTO> GetDailyReportById(Guid id, ThisUserObj currentUser);
        public Task<DailyReportDTO> GetDailyReportByDate(Guid id, string date, ThisUserObj currentUser);

        //UPDATE

        //DELETE
    }
}
