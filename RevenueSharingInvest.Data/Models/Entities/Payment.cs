using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Payment")]
    public partial class Payment
    {
        public Payment()
        {
            Transactions = new HashSet<Transaction>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("periodRevenueID")]
        [StringLength(10)]
        public string PeriodRevenueId { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("type")]
        [StringLength(20)]
        public string Type { get; set; }
        [Column("fromID")]
        [StringLength(10)]
        public string FromId { get; set; }
        [Column("toID")]
        [StringLength(10)]
        public string ToId { get; set; }
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

        [ForeignKey(nameof(PeriodRevenueId))]
        [InverseProperty("Payments")]
        public virtual PeriodRevenue PeriodRevenue { get; set; }
        [InverseProperty(nameof(Transaction.Payment))]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
