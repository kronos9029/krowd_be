﻿using System;
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
            Projects = new HashSet<Project>();
            Users = new HashSet<User>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(11)]
        public string PhoneNum { get; set; }
        public string Image { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        public string Description { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Address { get; set; }
        public int? NumOfProject { get; set; }
        public int? NumOfSuccessfulProject { get; set; }
        public double? SuccessfulRate { get; set; }
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [InverseProperty(nameof(BusinessField.Business))]
        public virtual ICollection<BusinessField> BusinessFields { get; set; }
        [InverseProperty(nameof(Project.Business))]
        public virtual ICollection<Project> Projects { get; set; }
        [InverseProperty(nameof(User.Business))]
        public virtual ICollection<User> Users { get; set; }
    }
}
