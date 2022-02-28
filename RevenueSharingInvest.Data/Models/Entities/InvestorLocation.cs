using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("InvestorLocation")]
    public partial class InvestorLocation
    {
        [Key]
        [Column("investorID")]
        [StringLength(10)]
        public string InvestorId { get; set; }
        [Key]
        [Column("areaID")]
        [StringLength(10)]
        public string AreaId { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("InvestorLocations")]
        public virtual Area Area { get; set; }
        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("InvestorLocations")]
        public virtual Investor Investor { get; set; }
    }
}
