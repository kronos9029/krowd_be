USE [KrowdDB]
GO
/****** Object:  Table [dbo].[AccountTransaction]    Script Date: 12/7/2022 5:04:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountTransaction](
	[Id] [uniqueidentifier] NOT NULL,
	[FromUserId] [uniqueidentifier] NULL,
	[ToUserId] [uniqueidentifier] NULL,
	[PartnerUserId] [nvarchar](max) NULL,
	[PartnerClientId] [uniqueidentifier] NULL,
	[Amount] [bigint] NOT NULL,
	[OrderType] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[PartnerCode] [nvarchar](max) NULL,
	[PayType] [nvarchar](max) NULL,
	[Signature] [nvarchar](max) NULL,
	[RequestId] [nvarchar](max) NULL,
	[ResponseTime] [bigint] NOT NULL,
	[ResultCode] [int] NOT NULL,
	[CallbackToken] [nvarchar](max) NULL,
	[ExtraData] [nvarchar](max) NULL,
	[OrderInfo] [nvarchar](max) NULL,
	[TransId] [bigint] NOT NULL,
	[CreateDate] [datetime] NULL,
	[Type] [nvarchar](max) NULL,
	[WithdrawRequestId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_AccountTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Area]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Area](
	[Id] [uniqueidentifier] NOT NULL,
	[City] [nvarchar](50) NULL,
	[District] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill](
	[Id] [uniqueidentifier] NOT NULL,
	[InvoiceId] [nvarchar](50) NULL,
	[DailyReportId] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateBy] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_Bill] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Business]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Business](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[PhoneNum] [nvarchar](11) NULL,
	[Image] [nvarchar](max) NULL,
	[Email] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[TaxIdentificationNumber] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[NumOfProject] [int] NULL,
	[NumOfSuccessfulProject] [int] NULL,
	[SuccessfulRate] [float] NULL,
	[Status] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Business] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BusinessField]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessField](
	[BusinessId] [uniqueidentifier] NOT NULL,
	[FieldId] [uniqueidentifier] NOT NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_BusinessField] PRIMARY KEY CLUSTERED 
(
	[BusinessId] ASC,
	[FieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DailyReport]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyReport](
	[Id] [uniqueidentifier] NOT NULL,
	[StageId] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[ReportDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[Status]  AS (case when dateadd(hour,(7),getdate())<[ReportDate] then 'UNDUE' when dateadd(hour,(7),getdate())>=[ReportDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'DUE' when [UpdateDate] IS NOT NULL AND [UpdateBy] IS NOT NULL then 'REPORTED' when dateadd(hour,(7),getdate())>dateadd(day,(1),[ReportDate]) AND [UpdateDate] IS NULL AND [UpdateBy] IS NULL then 'NOT_REPORTED'  end),
 CONSTRAINT [PK_DailyReport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Field]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Field](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Field] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Investment]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Investment](
	[Id] [uniqueidentifier] NOT NULL,
	[InvestorId] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[PackageId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NULL,
	[TotalPrice] [float] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[Status] [nvarchar](max) NULL,
	[Contract] [nvarchar](max) NULL,
 CONSTRAINT [PK_Investment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Investor]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Investor](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[InvestorTypeId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_Investor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvestorType]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvestorType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_InvestorType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvestorWallet]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvestorWallet](
	[Id] [uniqueidentifier] NOT NULL,
	[InvestorId] [uniqueidentifier] NULL,
	[Balance] [float] NULL,
	[WalletTypeId] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_InvestorWallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Package]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[Price] [float] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[RemainingQuantity] [int] NOT NULL,
	[Status]  AS (case when [dbo].[Get_Project_Status]([ProjectId])<>'CALLING_FOR_INVESTMENT' then 'INACTIVE' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]>(0) then 'IN_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])<[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) AND [RemainingQuantity]=(0) then 'OUT_OF_STOCK' when [dbo].[Get_Project_Status]([ProjectId])='CALLING_FOR_INVESTMENT' AND ([dbo].[Get_Project_InvestedCapital]([ProjectId])+[Price])>[dbo].[Get_Project_InvestmentTargetCapital]([ProjectId]) then 'BLOCKED' else 'INACTIVE' end),
 CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PackageVoucher]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageVoucher](
	[PackageId] [uniqueidentifier] NOT NULL,
	[VoucherId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NULL,
	[MaxQuantity] [int] NOT NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_PackageVoucher] PRIMARY KEY CLUSTERED 
(
	[PackageId] ASC,
	[VoucherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [uniqueidentifier] NOT NULL,
	[PeriodRevenueId] [uniqueidentifier] NULL,
	[InvestmentId] [uniqueidentifier] NULL,
	[Amount] [float] NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](20) NULL,
	[FromId] [uniqueidentifier] NULL,
	[ToId] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[Status] [nvarchar](max) NULL,
	[PackageId] [uniqueidentifier] NULL,
	[StageId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PeriodRevenue]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeriodRevenue](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[StageId] [uniqueidentifier] NOT NULL,
	[ActualAmount] [float] NULL,
	[SharedAmount] [float] NULL,
	[PaidAmount] [float] NULL,
	[PessimisticExpectedAmount] [float] NULL,
	[OptimisticExpectedAmount] [float] NULL,
	[NormalExpectedAmount] [float] NULL,
	[PessimisticExpectedRatio] [float] NULL,
	[OptimisticExpectedRatio] [float] NULL,
	[NormalExpectedRatio] [float] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[Status]  AS (case when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]>=[SharedAmount] then 'PAID_ENOUGH' when [ActualAmount] IS NOT NULL AND [SharedAmount] IS NOT NULL AND [PaidAmount] IS NOT NULL AND [PaidAmount]<[SharedAmount] then 'NOT_PAID_ENOUGH'  end),
 CONSTRAINT [PK_PeriodRevenue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PeriodRevenueHistory]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeriodRevenueHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[PeriodRevenueId] [uniqueidentifier] NULL,
	[Amount] [float] NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PeriodRevenueHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[BusinessId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[ManagerId] [uniqueidentifier] NOT NULL,
	[InvestmentTargetCapital] [float] NOT NULL,
	[InvestedCapital] [float] NOT NULL,
	[SharedRevenue] [float] NOT NULL,
	[Multiplier] [float] NOT NULL,
	[PaidAmount] [float] NOT NULL,
	[RemainingPayableAmount] [float] NOT NULL,
	[Duration] [int] NOT NULL,
	[NumOfStage] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[FieldId] [uniqueidentifier] NOT NULL,
	[AreaId] [uniqueidentifier] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[BusinessLicense] [nvarchar](13) NULL,
	[ApprovedDate] [datetime] NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[AccessKey] [nvarchar](16) NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectEntity]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectEntity](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[Content] [nvarchar](max) NULL,
	[Link] [nvarchar](max) NULL,
	[Priority] [int] NOT NULL,
 CONSTRAINT [PK_ProjectEntity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectWallet]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectWallet](
	[Id] [uniqueidentifier] NOT NULL,
	[WalletTypeId] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NULL,
	[ProjectManagerId] [uniqueidentifier] NOT NULL,
	[Balance] [float] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ProjectWallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Risk]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Risk](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ProjectId] [uniqueidentifier] NULL,
	[RiskTypeId] [uniqueidentifier] NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Risk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RiskType]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RiskType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_RiskType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stage]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stage](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[EndDate] [datetime] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[Status]  AS (case when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])>[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND [Name]=N'Giai đoạn thanh toán nợ' AND [dbo].[Get_Period_Revenue_Shared_Amount]([Id])<=[dbo].[Get_Period_Revenue_Paid_Amount]([Id]) then 'DONE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())<[EndDate] then 'UNDUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND (dateadd(hour,(7),getdate())>=[EndDate] AND dateadd(hour,(7),getdate())<=dateadd(day,(3),[EndDate])) then 'DUE' when [dbo].[Get_Project_Status]([ProjectId])='ACTIVE' AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'DONE' else 'INACTIVE' end),
	[IsOverDue]  AS (case when [dbo].[Get_Period_Revenue_History]([Id])>(0) AND [dbo].[Is_Paid_On_Stage]([Id])>(0) then 'FALSE' when [dbo].[Get_Period_Revenue_History]([Id])=(0) AND dateadd(hour,(7),getdate())>dateadd(day,(3),[EndDate]) then 'TRUE' when dateadd(hour,(7),getdate())<[EndDate] then NULL  end),
 CONSTRAINT [PK_Stage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWallet]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWallet](
	[Id] [uniqueidentifier] NOT NULL,
	[Balance] [float] NULL,
	[WalletTypeId] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_SystemWallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[BusinessId] [uniqueidentifier] NULL,
	[RoleId] [uniqueidentifier] NULL,
	[Description] [nvarchar](max) NULL,
	[LastName] [nvarchar](255) NULL,
	[FirstName] [nvarchar](255) NULL,
	[PhoneNum] [nvarchar](10) NULL,
	[Image] [nvarchar](max) NULL,
	[IdCard] [nvarchar](20) NULL,
	[Email] [nvarchar](50) NULL,
	[Gender] [nvarchar](10) NULL,
	[DateOfBirth] [nvarchar](20) NULL,
	[TaxIdentificationNumber] [nvarchar](20) NULL,
	[City] [nvarchar](255) NULL,
	[District] [nvarchar](255) NULL,
	[Address] [nvarchar](max) NULL,
	[BankName] [nvarchar](50) NULL,
	[BankAccount] [nvarchar](20) NULL,
	[Status] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[SecretKey] [nvarchar](32) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voucher]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NULL,
	[Name] [nvarchar](50) NULL,
	[Code] [nvarchar](10) NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[Status] [nvarchar](20) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Voucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherItem]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherItem](
	[Id] [uniqueidentifier] NOT NULL,
	[VoucherId] [uniqueidentifier] NULL,
	[InvestmentId] [uniqueidentifier] NULL,
	[IssuedDate] [datetime] NULL,
	[ExpireDate] [datetime] NULL,
	[RedeemDate] [datetime] NULL,
	[AvailableDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_VoucherItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WalletTransaction]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransaction](
	[Id] [uniqueidentifier] NOT NULL,
	[PaymentId] [uniqueidentifier] NULL,
	[SystemWalletId] [uniqueidentifier] NULL,
	[ProjectWalletId] [uniqueidentifier] NULL,
	[InvestorWalletId] [uniqueidentifier] NULL,
	[Amount] [float] NULL,
	[Fee] [float] NULL,
	[Description] [nvarchar](max) NULL,
	[FromWalletId] [uniqueidentifier] NULL,
	[ToWalletId] [uniqueidentifier] NULL,
	[Type] [nvarchar](20) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WalletTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WalletType]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Type] [nvarchar](10) NULL,
	[Description] [nvarchar](max) NULL,
	[Mode] [nvarchar](20) NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_WalletType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WithdrawRequest]    Script Date: 12/7/2022 5:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WithdrawRequest](
	[Id] [uniqueidentifier] NOT NULL,
	[BankName] [nvarchar](max) NOT NULL,
	[AccountName] [nvarchar](max) NOT NULL,
	[BankAccount] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Amount] [float] NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[RefusalReason] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateBy] [uniqueidentifier] NULL,
	[ReportMessage] [nvarchar](max) NULL,
	[FromWalletId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_WithdrawRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT (CONVERT([bigint],(0))) FOR [Amount]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT (CONVERT([bigint],(0))) FOR [ResponseTime]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT ((0)) FOR [ResultCode]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT (CONVERT([bigint],(0))) FOR [TransId]
GO
ALTER TABLE [dbo].[AccountTransaction] ADD  DEFAULT ('0001-01-01T00:00:00.000') FOR [CreateDate]
GO
ALTER TABLE [dbo].[Area] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Bill] ADD  CONSTRAINT [DF_Bill_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Business] ADD  CONSTRAINT [DF__Business__Id__3B75D760]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[DailyReport] ADD  CONSTRAINT [DF_DailyReport_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[DailyReport] ADD  CONSTRAINT [DF__DailyRepo__Amoun__3BFFE745]  DEFAULT ((0.0000000000000000e+000)) FOR [Amount]
GO
ALTER TABLE [dbo].[DailyReport] ADD  CONSTRAINT [DF__DailyRepo__Repor__3B0BC30C]  DEFAULT ('0001-01-01T00:00:00.000') FOR [ReportDate]
GO
ALTER TABLE [dbo].[Field] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Investment] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Investment] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [InvestorId]
GO
ALTER TABLE [dbo].[Investment] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ProjectId]
GO
ALTER TABLE [dbo].[Investment] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [PackageId]
GO
ALTER TABLE [dbo].[Investor] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[InvestorType] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[InvestorWallet] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Package] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Package] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [Price]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PeriodRevenue] ADD  CONSTRAINT [DF__PeriodRevenu__Id__0C85DE4D]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PeriodRevenue] ADD  CONSTRAINT [DF__PeriodRev__Proje__2CF2ADDF]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ProjectId]
GO
ALTER TABLE [dbo].[PeriodRevenue] ADD  CONSTRAINT [DF__PeriodRev__Stage__2BFE89A6]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [StageId]
GO
ALTER TABLE [dbo].[PeriodRevenueHistory] ADD  CONSTRAINT [DF__PeriodRevenu__Id__1EA48E88]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Id__619B8048]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Busines__4A8310C6]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [BusinessId]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Manager__51300E55]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ManagerId]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Investm__46B27FE2]  DEFAULT ((0.0000000000000000e+000)) FOR [InvestmentTargetCapital]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Investe__47A6A41B]  DEFAULT ((0.0000000000000000e+000)) FOR [InvestedCapital]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__SharedR__41EDCAC5]  DEFAULT ((0.0000000000000000e+000)) FOR [SharedRevenue]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Multipl__44CA3770]  DEFAULT ((0.0000000000000000e+000)) FOR [Multiplier]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Remaini__671F4F74]  DEFAULT ((0.0000000000000000e+000)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Remaini__681373AD]  DEFAULT ((0.0000000000000000e+000)) FOR [RemainingPayableAmount]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__Duratio__498EEC8D]  DEFAULT ((0)) FOR [Duration]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__NumOfSt__43D61337]  DEFAULT ((0)) FOR [NumOfStage]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__FieldId__5224328E]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [FieldId]
GO
ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF__Project__AreaId__531856C7]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [AreaId]
GO
ALTER TABLE [dbo].[ProjectEntity] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ProjectWallet] ADD  CONSTRAINT [DF__ProjectWalle__Id__693CA210]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Risk] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[RiskType] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Stage] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Stage] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ProjectId]
GO
ALTER TABLE [dbo].[SystemWallet] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[VoucherItem] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[WalletTransaction] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[WalletType] ADD  CONSTRAINT [DF__WalletType__Id__49C3F6B7]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[WithdrawRequest] ADD  CONSTRAINT [DF_WithdrawRequest_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[WithdrawRequest] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [FromWalletId]
GO
ALTER TABLE [dbo].[AccountTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AccountTransaction_User] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AccountTransaction] CHECK CONSTRAINT [FK_AccountTransaction_User]
GO
ALTER TABLE [dbo].[AccountTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AccountTransaction_User1] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AccountTransaction] CHECK CONSTRAINT [FK_AccountTransaction_User1]
GO
ALTER TABLE [dbo].[AccountTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AccountTransaction_WithdrawRequest] FOREIGN KEY([WithdrawRequestId])
REFERENCES [dbo].[WithdrawRequest] ([Id])
GO
ALTER TABLE [dbo].[AccountTransaction] CHECK CONSTRAINT [FK_AccountTransaction_WithdrawRequest]
GO
ALTER TABLE [dbo].[Bill]  WITH CHECK ADD  CONSTRAINT [FK_Bill_DailyReport] FOREIGN KEY([DailyReportId])
REFERENCES [dbo].[DailyReport] ([Id])
GO
ALTER TABLE [dbo].[Bill] CHECK CONSTRAINT [FK_Bill_DailyReport]
GO
ALTER TABLE [dbo].[BusinessField]  WITH CHECK ADD  CONSTRAINT [FK_BusinessField_Business] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Business] ([Id])
GO
ALTER TABLE [dbo].[BusinessField] CHECK CONSTRAINT [FK_BusinessField_Business]
GO
ALTER TABLE [dbo].[BusinessField]  WITH CHECK ADD  CONSTRAINT [FK_BusinessField_Field] FOREIGN KEY([FieldId])
REFERENCES [dbo].[Field] ([Id])
GO
ALTER TABLE [dbo].[BusinessField] CHECK CONSTRAINT [FK_BusinessField_Field]
GO
ALTER TABLE [dbo].[DailyReport]  WITH CHECK ADD  CONSTRAINT [FK_DailyReport_Stage] FOREIGN KEY([StageId])
REFERENCES [dbo].[Stage] ([Id])
GO
ALTER TABLE [dbo].[DailyReport] CHECK CONSTRAINT [FK_DailyReport_Stage]
GO
ALTER TABLE [dbo].[Investment]  WITH CHECK ADD  CONSTRAINT [FK_Investment_Investor] FOREIGN KEY([InvestorId])
REFERENCES [dbo].[Investor] ([Id])
GO
ALTER TABLE [dbo].[Investment] CHECK CONSTRAINT [FK_Investment_Investor]
GO
ALTER TABLE [dbo].[Investment]  WITH CHECK ADD  CONSTRAINT [FK_Investment_Package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Package] ([Id])
GO
ALTER TABLE [dbo].[Investment] CHECK CONSTRAINT [FK_Investment_Package]
GO
ALTER TABLE [dbo].[Investor]  WITH CHECK ADD  CONSTRAINT [FK_Investor_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Investor] CHECK CONSTRAINT [FK_Investor_User_UserId]
GO
ALTER TABLE [dbo].[InvestorWallet]  WITH CHECK ADD  CONSTRAINT [FK_InvestorWallet_Investor] FOREIGN KEY([InvestorId])
REFERENCES [dbo].[Investor] ([Id])
GO
ALTER TABLE [dbo].[InvestorWallet] CHECK CONSTRAINT [FK_InvestorWallet_Investor]
GO
ALTER TABLE [dbo].[InvestorWallet]  WITH CHECK ADD  CONSTRAINT [FK_InvestorWallet_WalletType] FOREIGN KEY([WalletTypeId])
REFERENCES [dbo].[WalletType] ([Id])
GO
ALTER TABLE [dbo].[InvestorWallet] CHECK CONSTRAINT [FK_InvestorWallet_WalletType]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_Project]
GO
ALTER TABLE [dbo].[PackageVoucher]  WITH CHECK ADD  CONSTRAINT [FK_PackageVoucher_Package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Package] ([Id])
GO
ALTER TABLE [dbo].[PackageVoucher] CHECK CONSTRAINT [FK_PackageVoucher_Package]
GO
ALTER TABLE [dbo].[PackageVoucher]  WITH CHECK ADD  CONSTRAINT [FK_PackageVoucher_Voucher] FOREIGN KEY([VoucherId])
REFERENCES [dbo].[Voucher] ([Id])
GO
ALTER TABLE [dbo].[PackageVoucher] CHECK CONSTRAINT [FK_PackageVoucher_Voucher]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Investment] FOREIGN KEY([InvestmentId])
REFERENCES [dbo].[Investment] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Investment]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_PeriodRevenue] FOREIGN KEY([PeriodRevenueId])
REFERENCES [dbo].[PeriodRevenue] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_PeriodRevenue]
GO
ALTER TABLE [dbo].[PeriodRevenue]  WITH CHECK ADD  CONSTRAINT [FK_PeriodRevenue_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[PeriodRevenue] CHECK CONSTRAINT [FK_PeriodRevenue_Project]
GO
ALTER TABLE [dbo].[PeriodRevenue]  WITH CHECK ADD  CONSTRAINT [FK_PeriodRevenue_Stage_StageId] FOREIGN KEY([StageId])
REFERENCES [dbo].[Stage] ([Id])
GO
ALTER TABLE [dbo].[PeriodRevenue] CHECK CONSTRAINT [FK_PeriodRevenue_Stage_StageId]
GO
ALTER TABLE [dbo].[PeriodRevenueHistory]  WITH CHECK ADD  CONSTRAINT [FK_PeriodRevenueHistory_PeriodRevenue] FOREIGN KEY([PeriodRevenueId])
REFERENCES [dbo].[PeriodRevenue] ([Id])
GO
ALTER TABLE [dbo].[PeriodRevenueHistory] CHECK CONSTRAINT [FK_PeriodRevenueHistory_PeriodRevenue]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Area]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Business] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Business] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Business]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_User] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_User]
GO
ALTER TABLE [dbo].[ProjectEntity]  WITH CHECK ADD  CONSTRAINT [FK_ProjectUpdate_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectEntity] CHECK CONSTRAINT [FK_ProjectUpdate_Project]
GO
ALTER TABLE [dbo].[ProjectWallet]  WITH CHECK ADD  CONSTRAINT [FK_BusinessWallet_WalletType] FOREIGN KEY([WalletTypeId])
REFERENCES [dbo].[WalletType] ([Id])
GO
ALTER TABLE [dbo].[ProjectWallet] CHECK CONSTRAINT [FK_BusinessWallet_WalletType]
GO
ALTER TABLE [dbo].[ProjectWallet]  WITH CHECK ADD  CONSTRAINT [FK_ProjectWallet_User] FOREIGN KEY([ProjectManagerId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProjectWallet] CHECK CONSTRAINT [FK_ProjectWallet_User]
GO
ALTER TABLE [dbo].[Risk]  WITH CHECK ADD  CONSTRAINT [FK_Risk_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[Risk] CHECK CONSTRAINT [FK_Risk_Project]
GO
ALTER TABLE [dbo].[Risk]  WITH CHECK ADD  CONSTRAINT [FK_Risk_RiskType] FOREIGN KEY([RiskTypeId])
REFERENCES [dbo].[RiskType] ([Id])
GO
ALTER TABLE [dbo].[Risk] CHECK CONSTRAINT [FK_Risk_RiskType]
GO
ALTER TABLE [dbo].[Stage]  WITH CHECK ADD  CONSTRAINT [FK_Stage_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[Stage] CHECK CONSTRAINT [FK_Stage_Project]
GO
ALTER TABLE [dbo].[SystemWallet]  WITH CHECK ADD  CONSTRAINT [FK_SystemWallet_WalletType] FOREIGN KEY([WalletTypeId])
REFERENCES [dbo].[WalletType] ([Id])
GO
ALTER TABLE [dbo].[SystemWallet] CHECK CONSTRAINT [FK_SystemWallet_WalletType]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Business] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Business] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Business]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role_RoleId]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_Project]
GO
ALTER TABLE [dbo].[VoucherItem]  WITH CHECK ADD  CONSTRAINT [FK_VoucherItem_Investment_InvestmentId] FOREIGN KEY([InvestmentId])
REFERENCES [dbo].[Investment] ([Id])
GO
ALTER TABLE [dbo].[VoucherItem] CHECK CONSTRAINT [FK_VoucherItem_Investment_InvestmentId]
GO
ALTER TABLE [dbo].[VoucherItem]  WITH CHECK ADD  CONSTRAINT [FK_VoucherItem_Voucher] FOREIGN KEY([VoucherId])
REFERENCES [dbo].[Voucher] ([Id])
GO
ALTER TABLE [dbo].[VoucherItem] CHECK CONSTRAINT [FK_VoucherItem_Voucher]
GO
ALTER TABLE [dbo].[WalletTransaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_InvestorWallet] FOREIGN KEY([InvestorWalletId])
REFERENCES [dbo].[InvestorWallet] ([Id])
GO
ALTER TABLE [dbo].[WalletTransaction] CHECK CONSTRAINT [FK_Transaction_InvestorWallet]
GO
ALTER TABLE [dbo].[WalletTransaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Payment] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payment] ([Id])
GO
ALTER TABLE [dbo].[WalletTransaction] CHECK CONSTRAINT [FK_Transaction_Payment]
GO
ALTER TABLE [dbo].[WalletTransaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_ProjectWallet] FOREIGN KEY([ProjectWalletId])
REFERENCES [dbo].[ProjectWallet] ([Id])
GO
ALTER TABLE [dbo].[WalletTransaction] CHECK CONSTRAINT [FK_Transaction_ProjectWallet]
GO
ALTER TABLE [dbo].[WalletTransaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_SystemWallet] FOREIGN KEY([SystemWalletId])
REFERENCES [dbo].[SystemWallet] ([Id])
GO
ALTER TABLE [dbo].[WalletTransaction] CHECK CONSTRAINT [FK_Transaction_SystemWallet]
GO
ALTER TABLE [dbo].[WithdrawRequest]  WITH CHECK ADD  CONSTRAINT [FK_WithdrawRequest_User] FOREIGN KEY([CreateBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[WithdrawRequest] CHECK CONSTRAINT [FK_WithdrawRequest_User]
GO
ALTER TABLE [dbo].[WithdrawRequest]  WITH CHECK ADD  CONSTRAINT [FK_WithdrawRequest_User1] FOREIGN KEY([UpdateBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[WithdrawRequest] CHECK CONSTRAINT [FK_WithdrawRequest_User1]
GO
