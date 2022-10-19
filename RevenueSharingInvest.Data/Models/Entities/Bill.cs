using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Bill")]
    public partial class Bill
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string InvoiceId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        [StringLength(50)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Bills")]
        public virtual Project Project { get; set; }
    }
}
