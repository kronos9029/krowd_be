﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Investment")]
    //[Index(nameof(InvestorId), Name = "IX_Investment_InvestorId")]
    //[Index(nameof(PackageId), Name = "IX_Investment_PackageId")]
    //[Index(nameof(ProjectId), Name = "IX_Investment_ProjectId")]
    public partial class Investment
    {
        public Investment()
        {
            Payments = new HashSet<Payment>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        [Key]
        public Guid Id { get; set; }
        //public Guid? InvestorId { get; set; }
        //public Guid? ProjectId { get; set; }
        //public Guid? PackageId { get; set; }
        public Guid InvestorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PackageId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        //[ForeignKey(nameof(InvestorId))]
        //[InverseProperty("Investments")]
        //public virtual Investor Investor { get; set; }
        //[ForeignKey(nameof(PackageId))]
        //[InverseProperty("Investments")]
        //public virtual Package Package { get; set; }
        //[ForeignKey(nameof(ProjectId))]
        //[InverseProperty("Investments")]
        //public virtual Project Project { get; set; }
        [InverseProperty(nameof(Payment.Investment))]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty(nameof(VoucherItem.Investment))]
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
