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
        public virtual DbSet<Data.Models.Entities.Business> Businesses { get; set; }
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
                entity.ToTable("AccountTransaction");

                entity.HasIndex(e => e.FromUserId, "IX_AccountTransaction_FromUserId");

                entity.HasIndex(e => e.ToUserId, "IX_AccountTransaction_ToUserId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Amount).HasDefaultValueSql("(CONVERT([bigint],(0)))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('0001-01-01T00:00:00.000')");

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
                entity.ToTable("Area");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceId).HasMaxLength(50);

                entity.HasOne(d => d.DailyReport)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.DailyReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bill_DailyReport");
            });

            modelBuilder.Entity<Data.Models.Entities.Business>(entity =>
            {
                entity.ToTable("Business");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PhoneNum).HasMaxLength(10);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BusinessField>(entity =>
            {
                entity.HasKey(e => new { e.BusinessId, e.FieldId });

                entity.ToTable("BusinessField");

                entity.HasIndex(e => e.FieldId, "IX_BusinessField_FieldId");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("DailyReport");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ReportDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.DailyReports)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DailyReport_Stage");
            });

            modelBuilder.Entity<Field>(entity =>
            {
                entity.ToTable("Field");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.ToTable("Investment");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Investor>(entity =>
            {
                entity.ToTable("Investor");

                entity.HasIndex(e => e.UserId, "IX_Investor_UserId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Investors)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<InvestorType>(entity =>
            {
                entity.ToTable("InvestorType");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InvestorWallet>(entity =>
            {
                entity.ToTable("InvestorWallet");

                entity.HasIndex(e => e.InvestorId, "IX_InvestorWallet_InvestorId");

                entity.HasIndex(e => e.WalletTypeId, "IX_InvestorWallet_WalletTypeId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("Package");

                entity.HasIndex(e => e.ProjectId, "IX_Package_ProjectId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' else 'INACTIVE' end)", false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Package_Project");
            });

            modelBuilder.Entity<PackageVoucher>(entity =>
            {
                entity.HasKey(e => new { e.PackageId, e.VoucherId });

                entity.ToTable("PackageVoucher");

                entity.HasIndex(e => e.VoucherId, "IX_PackageVoucher_VoucherId");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("Payment");

                entity.HasIndex(e => e.InvestmentId, "IX_Payment_InvestmentId");

                entity.HasIndex(e => e.PeriodRevenueId, "IX_Payment_PeriodRevenueId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(20);

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
                entity.ToTable("PeriodRevenue");

                entity.HasIndex(e => e.ProjectId, "IX_PeriodRevenue_ProjectId");

                entity.HasIndex(e => e.StageId, "IX_PeriodRevenue_StageId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("PeriodRevenueHistory");

                entity.HasIndex(e => e.PeriodRevenueId, "IX_PeriodRevenueHistory_PeriodRevenueId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.PeriodRevenue)
                    .WithMany(p => p.PeriodRevenueHistories)
                    .HasForeignKey(d => d.PeriodRevenueId)
                    .HasConstraintName("FK_PeriodRevenueHistory_PeriodRevenue");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.HasIndex(e => e.AreaId, "IX_Project_AreaId");

                entity.HasIndex(e => e.BusinessId, "IX_Project_BusinessId");

                entity.HasIndex(e => e.ManagerId, "IX_Project_ManagerId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccessKey).HasMaxLength(16);

                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

                entity.Property(e => e.BusinessLicense).HasMaxLength(13);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("ProjectEntity");

                entity.HasIndex(e => e.ProjectId, "IX_ProjectEntity_ProjectId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectEntities)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectUpdate_Project");
            });

            modelBuilder.Entity<ProjectWallet>(entity =>
            {
                entity.ToTable("ProjectWallet");

                entity.HasIndex(e => e.ProjectManagerId, "IX_ProjectWallet_ProjectId");

                entity.HasIndex(e => e.WalletTypeId, "IX_ProjectWallet_WalletTypeId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("Risk");

                entity.HasIndex(e => e.ProjectId, "IX_Risk_ProjectId");

                entity.HasIndex(e => e.RiskTypeId, "IX_Risk_RiskTypeId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
                entity.ToTable("RiskType");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.ToTable("Stage");

                entity.HasIndex(e => e.ProjectId, "IX_Stage_ProjectId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.IsOverDue)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Period_Revenue_History]([Id])<>(0) AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end)", false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[StartDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[StartDate] AND dateadd(hour,(7),getdate())<=[EndDate]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id])<>NULL then 'PAID' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>[EndDate] AND [dbo].[Get_Period_Revenue_Actual_Amount]([Id]) IS NULL then 'OVERDUE' else 'INACTIVE' end)", false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Stages)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stage_Project");
            });

            modelBuilder.Entity<SystemWallet>(entity =>
            {
                entity.ToTable("SystemWallet");

                entity.HasIndex(e => e.WalletTypeId, "IX_SystemWallet_WalletTypeId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.WalletType)
                    .WithMany(p => p.SystemWallets)
                    .HasForeignKey(d => d.WalletTypeId)
                    .HasConstraintName("FK_SystemWallet_WalletType");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.BusinessId, "IX_User_BusinessId");

                entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BankAccount).HasMaxLength(20);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasMaxLength(20);

                entity.Property(e => e.District).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.IdCard).HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.PhoneNum).HasMaxLength(10);

                entity.Property(e => e.SecretKey).HasMaxLength(32);

                entity.Property(e => e.TaxIdentificationNumber).HasMaxLength(20);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Business)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.BusinessId)
                    .HasConstraintName("FK_User_Business");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("Voucher");

                entity.HasIndex(e => e.ProjectId, "IX_Voucher_ProjectId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Voucher_Project");
            });

            modelBuilder.Entity<VoucherItem>(entity =>
            {
                entity.ToTable("VoucherItem");

                entity.HasIndex(e => e.InvestmentId, "IX_VoucherItem_InvestmentId");

                entity.HasIndex(e => e.VoucherId, "IX_VoucherItem_VoucherId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AvailableDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.IssuedDate).HasColumnType("datetime");

                entity.Property(e => e.RedeemDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.InvestmentId);

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK_VoucherItem_Voucher");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.ToTable("WalletTransaction");

                entity.HasIndex(e => e.InvestorWalletId, "IX_WalletTransaction_InvestorWalletId");

                entity.HasIndex(e => e.PaymentId, "IX_WalletTransaction_PaymentId");

                entity.HasIndex(e => e.ProjectWalletId, "IX_WalletTransaction_ProjectWalletId");

                entity.HasIndex(e => e.SystemWalletId, "IX_WalletTransaction_SystemWalletId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(20);

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
                entity.ToTable("WalletType");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Mode).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(10);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WithdrawRequest>(entity =>
            {
                entity.ToTable("WithdrawRequest");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccountName).IsRequired();

                entity.Property(e => e.BankAccount).IsRequired();

                entity.Property(e => e.BankName).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Status).IsRequired();

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

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
