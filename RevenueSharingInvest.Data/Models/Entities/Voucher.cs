using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Voucher
    {
        public Voucher()
        {
            PackageVouchers = new HashSet<PackageVoucher>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PackageVoucher> PackageVouchers { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
