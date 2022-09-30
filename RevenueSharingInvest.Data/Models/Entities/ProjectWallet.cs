using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("ProjectWallet")]
    [Index(nameof(ProjectManagerId), Name = "IX_ProjectWallet_ProjectId")]
    [Index(nameof(WalletTypeId), Name = "IX_ProjectWallet_WalletTypeId")]
    public partial class ProjectWallet
    {
        public ProjectWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public double? Balance { get; set; }
        public Guid? WalletTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey(nameof(ProjectManagerId))]
        [InverseProperty(nameof(User.ProjectWallets))]
        public virtual User ProjectManager { get; set; }
        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("ProjectWallets")]
        public virtual WalletType WalletType { get; set; }
        [InverseProperty(nameof(WalletTransaction.ProjectWallet))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
