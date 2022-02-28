using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RevenueSharingInvest.Data.Helpers
{
    public partial class KrowdContext : DbContext
    {
        public KrowdContext()
        {
        }

        public KrowdContext(DbContextOptions<KrowdContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Business> Businesses { get; set; }
        public virtual DbSet<BusinessField> BusinessFields { get; set; }
        public virtual DbSet<BusinessWallet> BusinessWallets { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Investment> Investments { get; set; }
        public virtual DbSet<Investor> Investors { get; set; }
        public virtual DbSet<InvestorLocation> InvestorLocations { get; set; }
        public virtual DbSet<InvestorType> InvestorTypes { get; set; }
        public virtual DbSet<InvestorWallet> InvestorWallets { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageVoucher> PackageVouchers { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PeriodRevenue> PeriodRevenues { get; set; }
        public virtual DbSet<PeriodRevenueHistory> PeriodRevenueHistories { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectHighlight> ProjectHighlights { get; set; }
        public virtual DbSet<ProjectUpdate> ProjectUpdates { get; set; }
        public virtual DbSet<Risk> Risks { get; set; }
        public virtual DbSet<RiskType> RiskTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Stage> Stages { get; set; }
        public virtual DbSet<SystemWallet> SystemWallets { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherItem> VoucherItems { get; set; }
        public virtual DbSet<WalletType> WalletTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=Krowd;User ID=sa;Password=123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BusinessField>(entity =>
            {
                entity.HasKey(e => new { e.BusinessId, e.FieldId });

                entity.HasOne(d => d.Business)
                    .WithMany(p => p.BusinessFields)
                    .HasForeignKey(d => d.BusinessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BusinessField_Business");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.BusinessFields)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BusinessField_Field");
            });

            modelBuilder.Entity<BusinessWallet>(entity =>
            {
                entity.HasOne(d => d.Business)
                    .WithMany(p => p.BusinessWallets)
                    .HasForeignKey(d => d.BusinessId)
                    .HasConstraintName("FK_BusinessWallet_Business");

                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.BusinessWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .HasConstraintName("FK_BusinessWallet_WalletType");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.InvestorId)
                    .HasConstraintName("FK_Investment_Investor");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("FK_Investment_Package");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Investment_Project");
            });

            modelBuilder.Entity<Investor>(entity =>
            {
                entity.HasOne(d => d.InvestorType)
                    .WithMany(p => p.Investors)
                    .HasForeignKey(d => d.InvestorTypeId)
                    .HasConstraintName("FK_Investor_InvestorType");
            });

            modelBuilder.Entity<InvestorLocation>(entity =>
            {
                entity.HasKey(e => new { e.InvestorId, e.AreaId });

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.InvestorLocations)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InvestorLocation_Area");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestorLocations)
                    .HasForeignKey(d => d.InvestorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InvestorLocation_Investor");
            });

            modelBuilder.Entity<InvestorWallet>(entity =>
            {
                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestorWallets)
                    .HasForeignKey(d => d.InvestorId)
                    .HasConstraintName("FK_InvestorWallet_Investor");

                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.InvestorWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .HasConstraintName("FK_InvestorWallet_WalletType");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Package_Project");
            });

            modelBuilder.Entity<PackageVoucher>(entity =>
            {
                entity.HasKey(e => new { e.PackageId, e.VoucherId });

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.PackageVouchers)
                    .HasForeignKey(d => d.PackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PackageVoucher_Package");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.PackageVouchers)
                    .HasForeignKey(d => d.VoucherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PackageVoucher_Voucher");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(d => d.PeriodRevenue)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PeriodRevenueId)
                    .HasConstraintName("FK_Payment_PeriodRevenue");
            });

            modelBuilder.Entity<PeriodRevenue>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.PeriodRevenues)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_PeriodRevenue_Project");
            });

            modelBuilder.Entity<PeriodRevenueHistory>(entity =>
            {
                entity.HasOne(d => d.PeriodRevenue)
                    .WithMany(p => p.PeriodRevenueHistories)
                    .HasForeignKey(d => d.PeriodRevenueId)
                    .HasConstraintName("FK_PeriodRevenueHistory_PeriodRevenue");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK_Project_Area");

                entity.HasOne(d => d.Business)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.BusinessId)
                    .HasConstraintName("FK_Project_Business");
            });

            modelBuilder.Entity<ProjectHighlight>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectHighlights)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectHighlight_Project");
            });

            modelBuilder.Entity<ProjectUpdate>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectUpdates)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectUpdate_Project");
            });

            modelBuilder.Entity<Risk>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Risk_Project");

                entity.HasOne(d => d.RiskType)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.RiskTypeId)
                    .HasConstraintName("FK_Risk_RiskType");
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Stages)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Stage_Project");
            });

            modelBuilder.Entity<SystemWallet>(entity =>
            {
                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.SystemWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .HasConstraintName("FK_SystemWallet_WalletType");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.InvestmentId)
                    .HasConstraintName("FK_Transaction_Investment");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Transaction_Payment");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Voucher_Project");
            });

            modelBuilder.Entity<VoucherItem>(entity =>
            {
                entity.HasKey(e => new { e.InvestorId, e.VoucherId });

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.InvestorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VoucherItem_Investor");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.VoucherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VoucherItem_Voucher");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
