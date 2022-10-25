using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RevenueSharingInvest.Data.Models.Entities;

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

        public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Models.Entities.Business> Businesses { get; set; }
        public virtual DbSet<BusinessField> BusinessFields { get; set; }
        public virtual DbSet<DailyReport> DailyReports { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Investment> Investments { get; set; }
        public virtual DbSet<Investor> Investors { get; set; }
        public virtual DbSet<InvestorType> InvestorTypes { get; set; }
        public virtual DbSet<InvestorWallet> InvestorWallets { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageVoucher> PackageVouchers { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PeriodRevenue> PeriodRevenues { get; set; }
        public virtual DbSet<PeriodRevenueHistory> PeriodRevenueHistories { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectEntity> ProjectEntities { get; set; }
        public virtual DbSet<ProjectWallet> ProjectWallets { get; set; }
        public virtual DbSet<Risk> Risks { get; set; }
        public virtual DbSet<RiskType> RiskTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Stage> Stages { get; set; }
        public virtual DbSet<SystemWallet> SystemWallets { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherItem> VoucherItems { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }
        public virtual DbSet<WalletType> WalletTypes { get; set; }
        public virtual DbSet<WithdrawRequest> WithdrawRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=Krowd;User ID=sa;Password=123");
                //optionsBuilder.UseSqlServer("Data Source=krowddb.cn4oiq8oeltn.ap-southeast-1.rds.amazonaws.com;Initial Catalog=KrowdDB;User ID=krowdAdmin2022;Password=krowd2022");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AccountTransaction>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Amount).HasDefaultValueSql("(CONVERT([bigint],(0)))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("('0001-01-01T00:00:00.000')");

                entity.Property(e => e.ResponseTime).HasDefaultValueSql("(CONVERT([bigint],(0)))");

                entity.Property(e => e.TransId).HasDefaultValueSql("(CONVERT([bigint],(0)))");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.AccountTransactionFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_AccountTransaction_User");

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.AccountTransactionToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_AccountTransaction_User1");

                entity.HasOne(d => d.WithdrawRequest)
                    .WithMany(p => p.AccountTransactions)
                    .HasForeignKey(d => d.WithdrawRequestId)
                    .HasConstraintName("FK_AccountTransaction_WithdrawRequest");
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.DailyReport)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.DailyReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bill_DailyReport");
            });

            modelBuilder.Entity<Models.Entities.Business>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

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

            modelBuilder.Entity<DailyReport>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ReportDate).HasDefaultValueSql("('0001-01-01T00:00:00.000')");

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when dateadd(hour,(7),getdate())<[ReportDate] then 'UNDUE' when dateadd(hour,(7),getdate())>=[ReportDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'DUE' when [UpdateDate] IS NOT NULL AND [UpdateBy] IS NOT NULL then 'REPORTED' when dateadd(hour,(7),getdate())>dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'NOT_REPORTED'  end)", false);

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.DailyReports)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DailyReport_Stage");
            });

            modelBuilder.Entity<Field>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Investor>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<InvestorType>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<InvestorWallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

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
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)", false);

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
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.InvestmentId)
                    .HasConstraintName("FK_Payment_Investment");

                entity.HasOne(d => d.PeriodRevenue)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PeriodRevenueId)
                    .HasConstraintName("FK_Payment_PeriodRevenue");
            });

            modelBuilder.Entity<PeriodRevenue>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.PaidAmount).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]>=[SharedAmount] then 'PAID_ENOUGH' when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]<[SharedAmount] then 'NOT_PAID_ENOUGH'  end)", false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.PeriodRevenues)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodRevenue_Project");

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.PeriodRevenues)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PeriodRevenueHistory>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.PeriodRevenue)
                    .WithMany(p => p.PeriodRevenueHistories)
                    .HasForeignKey(d => d.PeriodRevenueId)
                    .HasConstraintName("FK_PeriodRevenueHistory_PeriodRevenue");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK_Project_Area");

                entity.HasOne(d => d.Business)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.BusinessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_Business");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK_Project_User");
            });

            modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectEntities)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectUpdate_Project");
            });

            modelBuilder.Entity<ProjectWallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.ProjectManager)
                    .WithMany(p => p.ProjectWallets)
                    .HasForeignKey(d => d.ProjectManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectWallet_User");

                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.ProjectWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BusinessWallet_WalletType");
            });

            modelBuilder.Entity<Risk>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Risk_Project");

                entity.HasOne(d => d.RiskType)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.RiskTypeId)
                    .HasConstraintName("FK_Risk_RiskType");
            });

            modelBuilder.Entity<RiskType>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsOverDue)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)", false);

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end)", false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Stages)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stage_Project");
            });

            modelBuilder.Entity<SystemWallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.SystemWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .HasConstraintName("FK_SystemWallet_WalletType");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Business)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.BusinessId)
                    .HasConstraintName("FK_User_Business");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Voucher_Project");
            });

            modelBuilder.Entity<VoucherItem>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK_VoucherItem_Voucher");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.InvestorWallet)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.InvestorWalletId)
                    .HasConstraintName("FK_Transaction_InvestorWallet");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Transaction_Payment");

                entity.HasOne(d => d.ProjectWallet)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.ProjectWalletId)
                    .HasConstraintName("FK_Transaction_ProjectWallet");

                entity.HasOne(d => d.SystemWallet)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.SystemWalletId)
                    .HasConstraintName("FK_Transaction_SystemWallet");
            });

            modelBuilder.Entity<WalletType>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<WithdrawRequest>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.CreateByNavigation)
                    .WithMany(p => p.WithdrawRequestCreateByNavigations)
                    .HasForeignKey(d => d.CreateBy)
                    .HasConstraintName("FK_WithdrawRequest_User");

                entity.HasOne(d => d.UpdateByNavigation)
                    .WithMany(p => p.WithdrawRequestUpdateByNavigations)
                    .HasForeignKey(d => d.UpdateBy)
                    .HasConstraintName("FK_WithdrawRequest_User1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
