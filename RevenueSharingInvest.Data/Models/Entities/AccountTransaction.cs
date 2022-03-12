using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("AccountTransaction")]
    public partial class AccountTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(10)]
        public string UpdateBy { get; set; }
        [StringLength(10)]
        public string Status { get; set; }

        [ForeignKey(nameof(FromUserId))]
        [InverseProperty(nameof(User.AccountTransactionFromUsers))]
        public virtual User FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        [InverseProperty(nameof(User.AccountTransactionToUsers))]
        public virtual User ToUser { get; set; }
    }
}
