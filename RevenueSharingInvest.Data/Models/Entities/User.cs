using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            AccountTransactionFromUsers = new HashSet<AccountTransaction>();
            AccountTransactionToUsers = new HashSet<AccountTransaction>();
            Projects = new HashSet<Project>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? BusinessId { get; set; }
        public Guid? InvestorId { get; set; }
        public Guid? RoleId { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
        public string LastName { get; set; }
        [StringLength(20)]
        public string FirstName { get; set; }
        public string Status { get; set; }
        [StringLength(11)]
        public string PhoneNum { get; set; }
        public string Image { get; set; }
        [StringLength(20)]
        public string IdCard { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(10)]
        public string Gender { get; set; }
        [StringLength(20)]
        public string DateOfBirth { get; set; }
        [StringLength(20)]
        public string TaxIdentificationNumber { get; set; }
        [StringLength(20)]
        public string City { get; set; }
        [StringLength(20)]
        public string District { get; set; }
        [StringLength(20)]
        public string Ward { get; set; }
        public string Address { get; set; }
        [StringLength(50)]
        public string BankName { get; set; }
        [StringLength(20)]
        public string BankAccount { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(BusinessId))]
        [InverseProperty("Users")]
        public virtual Business Business { get; set; }
        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("Users")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(InvestorId))]
        [InverseProperty(nameof(Role.Users))]
        public virtual Role InvestorNavigation { get; set; }
        [InverseProperty(nameof(AccountTransaction.FromUser))]
        public virtual ICollection<AccountTransaction> AccountTransactionFromUsers { get; set; }
        [InverseProperty(nameof(AccountTransaction.ToUser))]
        public virtual ICollection<AccountTransaction> AccountTransactionToUsers { get; set; }
        [InverseProperty(nameof(Project.Manager))]
        public virtual ICollection<Project> Projects { get; set; }
    }
}
