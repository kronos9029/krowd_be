using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Business")]
    public partial class Business
    {
        public Business()
        {
            BusinessFields = new HashSet<BusinessField>();
            BusinessWallets = new HashSet<BusinessWallet>();
            Projects = new HashSet<Project>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("phoneNum")]
        [StringLength(10)]
        public string PhoneNum { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("email")]
        [StringLength(50)]
        public string Email { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("taxIdentificationNumber")]
        public string TaxIdentificationNumber { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("bank")]
        [StringLength(50)]
        public string Bank { get; set; }
        [Column("bankAccount")]
        [StringLength(14)]
        public string BankAccount { get; set; }
        [Column("numOfProject")]
        public int? NumOfProject { get; set; }
        [Column("numOfSuccessfulProject")]
        public int? NumOfSuccessfulProject { get; set; }
        [Column("successfulRate")]
        public double? SuccessfulRate { get; set; }
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

        [InverseProperty(nameof(BusinessField.Business))]
        public virtual ICollection<BusinessField> BusinessFields { get; set; }
        [InverseProperty(nameof(BusinessWallet.Business))]
        public virtual ICollection<BusinessWallet> BusinessWallets { get; set; }
        [InverseProperty(nameof(Project.Business))]
        public virtual ICollection<Project> Projects { get; set; }
    }
}
