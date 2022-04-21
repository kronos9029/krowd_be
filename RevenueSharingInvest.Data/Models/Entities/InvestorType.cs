using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("InvestorType")]
    public partial class InvestorType
    {
        public InvestorType()
        {
            Investors = new HashSet<Investor>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public long? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public long? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(Investor.InvestorType))]
        public virtual ICollection<Investor> Investors { get; set; }
    }
}
