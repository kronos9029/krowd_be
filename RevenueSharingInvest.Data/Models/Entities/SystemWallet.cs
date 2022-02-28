using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("SystemWallet")]
    public partial class SystemWallet
    {
        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
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
        [Column("updateBy")]
        [StringLength(10)]
        public string UpdateBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("SystemWallets")]
        public virtual WalletType WalletType { get; set; }
    }
}
