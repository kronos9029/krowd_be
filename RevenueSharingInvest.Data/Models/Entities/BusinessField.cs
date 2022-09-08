using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("BusinessField")]
    [Index(nameof(FieldId), Name = "IX_BusinessField_FieldId")]
    public partial class BusinessField
    {
        [Key]
        public Guid BusinessId { get; set; }
        [Key]
        public Guid FieldId { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(BusinessId))]
        [InverseProperty("BusinessFields")]
        public virtual Business Business { get; set; }
        [ForeignKey(nameof(FieldId))]
        [InverseProperty("BusinessFields")]
        public virtual Field Field { get; set; }
    }
}
