using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class DailyReportDTO
    {
        public string id { get; set; }
        public string stageId { get; set; }
        public double amount { get; set; }
        public string reportDate { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public string status { get; set; }
    }
}
