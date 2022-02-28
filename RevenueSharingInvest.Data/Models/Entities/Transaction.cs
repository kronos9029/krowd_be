using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Transaction")]
    public partial class Transaction
    {
        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("paymentID")]
        [StringLength(10)]
        public string PaymentId { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("type")]
        [StringLength(20)]
        public string Type { get; set; }
        [Column("fromWalletID")]
        [StringLength(10)]
        public string FromWalletId { get; set; }
        [Column("toWalletID")]
        [StringLength(10)]
        public string ToWalletId { get; set; }
        [Column("fee")]
        public double? Fee { get; set; }
        [Column("createDate", TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column("createBy")]
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column("updateDate", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("updateBy")]
        [StringLength(10)]
        public string UpdateBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }
        [Column("investmentID")]
        [StringLength(10)]
        public string InvestmentId { get; set; }

        [ForeignKey(nameof(InvestmentId))]
        [InverseProperty("Transactions")]
        public virtual Investment Investment { get; set; }
        [ForeignKey(nameof(PaymentId))]
        [InverseProperty("Transactions")]
        public virtual Payment Payment { get; set; }
    }
}
