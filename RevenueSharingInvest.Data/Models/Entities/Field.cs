using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Field")]
    public partial class Field
    {
        public Field()
        {
            BusinessFields = new HashSet<BusinessField>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(BusinessField.Field))]
        public virtual ICollection<BusinessField> BusinessFields { get; set; }
    }
}
