using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Project
    {
        public Project()
        {
            Bills = new HashSet<Bill>();
            Packages = new HashSet<Package>();
            PeriodRevenues = new HashSet<PeriodRevenue>();
            ProjectEntities = new HashSet<ProjectEntity>();
            Risks = new HashSet<Risk>();
            Stages = new HashSet<Stage>();
            Vouchers = new HashSet<Voucher>();
        }

        public Guid Id { get; set; }
        public Guid ManagerId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid FieldId { get; set; }
        public Guid AreaId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double InvestmentTargetCapital { get; set; }
        public double InvestedCapital { get; set; }
        public double SharedRevenue { get; set; }
        public double Multiplier { get; set; }
        public int Duration { get; set; }
        public int NumOfStage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BusinessLicense { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public string Status { get; set; }
        public string AccessKey { get; set; }
        public double RemainingMaximumPayableAmount { get; set; }
        public double RemainingPayableAmount { get; set; }

        public virtual Area Area { get; set; }
        public virtual Business Business { get; set; }
        public virtual User Manager { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
        public virtual ICollection<ProjectEntity> ProjectEntities { get; set; }
        public virtual ICollection<Risk> Risks { get; set; }
        public virtual ICollection<Stage> Stages { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
