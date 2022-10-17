using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class PackageVoucher
    {
        public Guid PackageId { get; set; }
        public Guid VoucherId { get; set; }
        public int? Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual Package Package { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
