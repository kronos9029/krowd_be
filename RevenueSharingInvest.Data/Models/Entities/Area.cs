using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Area
    {
        public Area()
        {
            Projects = new HashSet<Project>();
        }

        public Guid Id { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
