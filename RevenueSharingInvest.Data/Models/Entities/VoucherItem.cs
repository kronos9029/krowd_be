using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class VoucherItem
    {
        public Guid Id { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? InvestmentId { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? RedeemDate { get; set; }
        public DateTime? AvailableDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual Investment Investment { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
