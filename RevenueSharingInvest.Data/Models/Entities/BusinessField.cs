using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class BusinessField
    {
        public Guid BusinessId { get; set; }
        public Guid FieldId { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual Business Business { get; set; }
        public virtual Field Field { get; set; }
    }
}
