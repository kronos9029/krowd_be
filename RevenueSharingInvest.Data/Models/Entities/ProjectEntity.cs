using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class ProjectEntity
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public int Priority { get; set; }

        public virtual Project Project { get; set; }
    }
}
