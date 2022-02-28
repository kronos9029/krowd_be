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
            InvestorLocations = new HashSet<InvestorLocation>();
            InvestorWallets = new HashSet<InvestorWallet>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("userID")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Column("lastName")]
        [StringLength(20)]
        public string LastName { get; set; }
        [Column("firstName")]
        [StringLength(20)]
        public string FirstName { get; set; }
        [Column("phoneNum")]
        [StringLength(10)]
        public string PhoneNum { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("IDCard")]
        [StringLength(20)]
        public string Idcard { get; set; }
        [Column("email")]
        [StringLength(50)]
        public string Email { get; set; }
        [Column("gender")]
        [StringLength(10)]
        public string Gender { get; set; }
        [Column("dateOfBirth")]
        [StringLength(20)]
        public string DateOfBirth { get; set; }
        [Column("taxIdentificationNumber")]
        [StringLength(13)]
        public string TaxIdentificationNumber { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("bank")]
        [StringLength(50)]
        public string Bank { get; set; }
        [Column("bankAccount")]
        [StringLength(20)]
        public string BankAccount { get; set; }
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
        [Column("investorTypeID")]
        [StringLength(10)]
        public string InvestorTypeId { get; set; }

        [ForeignKey(nameof(InvestorTypeId))]
        [InverseProperty("Investors")]
        public virtual InvestorType InvestorType { get; set; }
        [InverseProperty(nameof(Investment.Investor))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(InvestorLocation.Investor))]
        public virtual ICollection<InvestorLocation> InvestorLocations { get; set; }
        [InverseProperty(nameof(InvestorWallet.Investor))]
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
        [InverseProperty(nameof(VoucherItem.Investor))]
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
