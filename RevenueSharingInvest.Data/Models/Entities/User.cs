using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class User
    {
        public User()
        {
            AccountTransactionFromUsers = new HashSet<AccountTransaction>();
            AccountTransactionToUsers = new HashSet<AccountTransaction>();
            Investors = new HashSet<Investor>();
            ProjectWallets = new HashSet<ProjectWallet>();
            Projects = new HashSet<Project>();
        }

        public Guid Id { get; set; }
        public Guid? BusinessId { get; set; }
        public Guid? RoleId { get; set; }
        public string Description { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNum { get; set; }
        public string Image { get; set; }
        public string IdCard { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public string SecretKey { get; set; }

        public virtual Business Business { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<AccountTransaction> AccountTransactionFromUsers { get; set; }
        public virtual ICollection<AccountTransaction> AccountTransactionToUsers { get; set; }
        public virtual ICollection<Investor> Investors { get; set; }
        public virtual ICollection<ProjectWallet> ProjectWallets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
