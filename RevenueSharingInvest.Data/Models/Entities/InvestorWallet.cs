using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("InvestorWallet")]
    public partial class InvestorWallet
    {
        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("investorID")]
        [StringLength(10)]
        public string InvestorId { get; set; }
        [Column("balance")]
        public double? Balance { get; set; }
        [Column("walletTypeID")]
        [StringLength(10)]
        public string WalletTypeId { get; set; }
        [Column("createDate", TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column("createBy")]
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column("updateDate", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("upDateBy")]
        [StringLength(10)]
        public string UpDateBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("InvestorWallets")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("InvestorWallets")]
        public virtual WalletType WalletType { get; set; }
    }
}
