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
            Users = new HashSet<User>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(10)]
        public string UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? InvestorTypeId { get; set; }

        [ForeignKey(nameof(InvestorTypeId))]
        [InverseProperty("Investors")]
        public virtual InvestorType InvestorType { get; set; }
        [InverseProperty(nameof(Investment.Investor))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(InvestorWallet.Investor))]
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
        [InverseProperty(nameof(User.Investor))]
        public virtual ICollection<User> Users { get; set; }
    }
}
