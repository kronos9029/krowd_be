using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("BusinessField")]
    public partial class BusinessField
    {
        [Key]
        public Guid BusinessId { get; set; }
        [Key]
        public Guid FieldId { get; set; }

        [ForeignKey(nameof(BusinessId))]
        [InverseProperty("BusinessFields")]
        public virtual Business Business { get; set; }
        [ForeignKey(nameof(FieldId))]
        [InverseProperty("BusinessFields")]
        public virtual Field Field { get; set; }
    }
}
