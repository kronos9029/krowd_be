using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Package
    {
        public Package()
        {
            PackageVouchers = new HashSet<PackageVoucher>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public int RemainingQuantity { get; set; }
        public string Status { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PackageVoucher> PackageVouchers { get; set; }
    }
}
