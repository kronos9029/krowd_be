using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Risk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? RiskTypeId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual Project Project { get; set; }
        public virtual RiskType RiskType { get; set; }
    }
}
