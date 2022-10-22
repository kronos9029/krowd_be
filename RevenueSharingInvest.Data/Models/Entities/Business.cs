using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Business
    {
        public Business()
        {
            BusinessFields = new HashSet<BusinessField>();
            Projects = new HashSet<Project>();
            Users = new HashSet<User>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNum { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Address { get; set; }
        public int? NumOfProject { get; set; }
        public int? NumOfSuccessfulProject { get; set; }
        public double? SuccessfulRate { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<BusinessField> BusinessFields { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
