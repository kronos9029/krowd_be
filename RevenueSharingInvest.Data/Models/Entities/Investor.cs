using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Investor")]
    public partial class Investor
    {
        public Investor()
        {
            Investments = new HashSet<Investment>();
            InvestorWallets = new HashSet<InvestorWallet>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? InvestorTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestorTypeId))]
        [InverseProperty("Investors")]
        public virtual InvestorType InvestorType { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Investors")]
        public virtual User User { get; set; }
        [InverseProperty(nameof(Investment.Investor))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(InvestorWallet.Investor))]
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
    }
}
