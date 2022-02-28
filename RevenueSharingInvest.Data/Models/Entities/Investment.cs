using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Investment")]
    public partial class Investment
    {
        public Investment()
        {
            Transactions = new HashSet<Transaction>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("investorID")]
        [StringLength(10)]
        public string InvestorId { get; set; }
        [Column("projectID")]
        [StringLength(10)]
        public string ProjectId { get; set; }
        [Column("packageID")]
        [StringLength(10)]
        public string PackageId { get; set; }
        [Column("quantity")]
        public int? Quantity { get; set; }
        [Column("totalPrice")]
        public double? TotalPrice { get; set; }
        [Column("lastPayment", TypeName = "datetime")]
        public DateTime? LastPayment { get; set; }
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

        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("Investments")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(PackageId))]
        [InverseProperty("Investments")]
        public virtual Package Package { get; set; }
        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Investments")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(Transaction.Investment))]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
