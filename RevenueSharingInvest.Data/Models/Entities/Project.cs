using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Project")]
    [Index(nameof(AreaId), Name = "IX_Project_AreaId")]
    [Index(nameof(BusinessId), Name = "IX_Project_BusinessId")]
    [Index(nameof(ManagerId), Name = "IX_Project_ManagerId")]
    public partial class Project
    {
        public Project()
        {
            Packages = new HashSet<Package>();
            PeriodRevenues = new HashSet<PeriodRevenue>();
            ProjectEntities = new HashSet<ProjectEntity>();
            Risks = new HashSet<Risk>();
            Stages = new HashSet<Stage>();
            Vouchers = new HashSet<Voucher>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public Guid BusinessId { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public Guid ManagerId { get; set; }
        public double InvestmentTargetCapital { get; set; }
        public double InvestedCapital { get; set; }
        public double SharedRevenue { get; set; }
        public double Multiplier { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingPayableAmount { get; set; }
        public int Duration { get; set; }
        public int NumOfStage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        public Guid FieldId { get; set; }
        public Guid AreaId { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        [StringLength(13)]
        public string BusinessLicense { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [StringLength(16)]
        public string AccessKey { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("Projects")]
        public virtual Area Area { get; set; }
        [ForeignKey(nameof(BusinessId))]
        [InverseProperty("Projects")]
        public virtual Business Business { get; set; }
        [ForeignKey(nameof(ManagerId))]
        [InverseProperty(nameof(User.Projects))]
        public virtual User Manager { get; set; }
        [InverseProperty(nameof(Package.Project))]
        public virtual ICollection<Package> Packages { get; set; }
        [InverseProperty(nameof(PeriodRevenue.Project))]
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
        [InverseProperty(nameof(ProjectEntity.Project))]
        public virtual ICollection<ProjectEntity> ProjectEntities { get; set; }
        [InverseProperty(nameof(Risk.Project))]
        public virtual ICollection<Risk> Risks { get; set; }
        [InverseProperty(nameof(Stage.Project))]
        public virtual ICollection<Stage> Stages { get; set; }
        [InverseProperty(nameof(Voucher.Project))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
