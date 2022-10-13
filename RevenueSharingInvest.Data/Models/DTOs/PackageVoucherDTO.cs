using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PackageVoucherDTO
    {
        public string packageId { get; set; }
        public string voucherId { get; set; }
        public int quantity { get; set; }
        public int maxQuantity { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}
