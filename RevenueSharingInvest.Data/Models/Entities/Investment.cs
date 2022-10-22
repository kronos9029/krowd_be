using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Investment
    {
        public Investment()
        {
            Payments = new HashSet<Payment>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        public Guid Id { get; set; }
        public Guid InvestorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PackageId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
