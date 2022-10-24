using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class BillDTO
    {
        public string invoiceId { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
        public string createBy { get; set; }
        public string createDate { get; set; }
    }

    public class InsertBillDTO
    {
        public string projectId { get; set; }
        public string dailyReportId { get; set; }
        public List<BillDTO> bills { get; set; }
    }

    public class GetBillDTO : BillDTO
    {
        public string id { get; set; }
        public string dailyReportId { get; set; }
    }
}
