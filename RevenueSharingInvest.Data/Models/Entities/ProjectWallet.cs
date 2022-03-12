using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("ProjectWallet")]
    public partial class ProjectWallet
    {
        public ProjectWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public double? Balance { get; set; }
        public Guid? WalletTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(10)]
        public string UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("ProjectWallets")]
        public virtual Project Project { get; set; }
        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("ProjectWallets")]
        public virtual WalletType WalletType { get; set; }
        [InverseProperty(nameof(WalletTransaction.ProjectWallet))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
