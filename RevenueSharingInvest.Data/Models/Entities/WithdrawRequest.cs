using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("WithdrawRequest")]
    public partial class WithdrawRequest
    {
        public WithdrawRequest()
        {
            AccountTransactions = new HashSet<AccountTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string BankAccount { get; set; }
        [Required]
        public string Description { get; set; }
        public string ReportMessage { get; set; }
        public double Amount { get; set; }
        [Required]
        public string Status { get; set; }
        public string RefusalReason { get; set; }
        public Guid FromWalletId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey(nameof(CreateBy))]
        [InverseProperty(nameof(User.WithdrawRequestCreateByNavigations))]
        public virtual User CreateByNavigation { get; set; }
        [ForeignKey(nameof(UpdateBy))]
        [InverseProperty(nameof(User.WithdrawRequestUpdateByNavigations))]
        public virtual User UpdateByNavigation { get; set; }
        [InverseProperty(nameof(AccountTransaction.WithdrawRequest))]
        public virtual ICollection<AccountTransaction> AccountTransactions { get; set; }
    }
}
