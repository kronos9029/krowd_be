using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllDailyReportDTO
    {
        public int numOfDailyReport { get; set; }
        public List<DailyReportDTO> listOfDailyReport { get; set; }
    }
}
