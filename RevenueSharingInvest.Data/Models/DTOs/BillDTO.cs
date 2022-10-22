using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class BillDTO
    {
        public string InvoiceId { get; set; }
        public string DailyReportId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public string CreateDate { get; set; }
    }

    public class InsertBillDTO
    {
        public string ProjectId { get; set; }
        public List<BillDTO> Bills { get; set; }
    }
}
