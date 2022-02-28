using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Project")]
    public partial class Project
    {
        public Project()
        {
            Investments = new HashSet<Investment>();
            Packages = new HashSet<Package>();
            PeriodRevenues = new HashSet<PeriodRevenue>();
            ProjectHighlights = new HashSet<ProjectHighlight>();
            ProjectUpdates = new HashSet<ProjectUpdate>();
            Risks = new HashSet<Risk>();
            Stages = new HashSet<Stage>();
            Vouchers = new HashSet<Voucher>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("businessID")]
        [StringLength(10)]
        public string BusinessId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("category")]
        [StringLength(20)]
        public string Category { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("areaID")]
        [StringLength(10)]
        public string AreaId { get; set; }
        [Column("investmentTargetCapital")]
        public double? InvestmentTargetCapital { get; set; }
        [Column("investedCapital")]
        public double? InvestedCapital { get; set; }
        [Column("sharedRevenue")]
        public double? SharedRevenue { get; set; }
        [Column("multiplier")]
        public double? Multiplier { get; set; }
        [Column("duration")]
        public int? Duration { get; set; }
        [Column("numOfPeriod")]
        public int? NumOfPeriod { get; set; }
        [Column("minInvestmentAmount")]
        public double? MinInvestmentAmount { get; set; }
        [Column("startDate", TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column("endDate", TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Column("businessLicense")]
        [StringLength(13)]
        public string BusinessLicense { get; set; }
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; }
        [Column("createDate", TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column("createBy")]
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column("updateDate", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("updateBy")]
        [StringLength(10)]
        public string UpdateBy { get; set; }
        [Column("approvedDate", TypeName = "datetime")]
        public DateTime? ApprovedDate { get; set; }
        [Column("approvedBy")]
        [StringLength(10)]
        public string ApprovedBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("Projects")]
        public virtual Area Area { get; set; }
        [ForeignKey(nameof(BusinessId))]
        [InverseProperty("Projects")]
        public virtual Business Business { get; set; }
        [InverseProperty(nameof(Investment.Project))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(Package.Project))]
        public virtual ICollection<Package> Packages { get; set; }
        [InverseProperty(nameof(PeriodRevenue.Project))]
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
        [InverseProperty(nameof(ProjectHighlight.Project))]
        public virtual ICollection<ProjectHighlight> ProjectHighlights { get; set; }
        [InverseProperty(nameof(ProjectUpdate.Project))]
        public virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; }
        [InverseProperty(nameof(Risk.Project))]
        public virtual ICollection<Risk> Risks { get; set; }
        [InverseProperty(nameof(Stage.Project))]
        public virtual ICollection<Stage> Stages { get; set; }
        [InverseProperty(nameof(Voucher.Project))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
