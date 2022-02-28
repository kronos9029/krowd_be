using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RevenueSharingInvest.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    district = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ward = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Business",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phoneNum = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    taxIdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    bankAccount = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    numOfProject = table.Column<int>(type: "int", nullable: true),
                    numOfSuccessfulProject = table.Column<int>(type: "int", nullable: true),
                    successfulRate = table.Column<double>(type: "float", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InvestorType",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RiskType",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WalletType",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    businessID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    areaID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    investmentTargetCapital = table.Column<double>(type: "float", nullable: true),
                    investedCapital = table.Column<double>(type: "float", nullable: true),
                    sharedRevenue = table.Column<double>(type: "float", nullable: true),
                    multiplier = table.Column<double>(type: "float", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    numOfPeriod = table.Column<int>(type: "int", nullable: true),
                    minInvestmentAmount = table.Column<double>(type: "float", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    businessLicense = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    approvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    approvedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Project_Area",
                        column: x => x.areaID,
                        principalTable: "Area",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Business",
                        column: x => x.businessID,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessField",
                columns: table => new
                {
                    businessID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    fieldID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessField", x => new { x.businessID, x.fieldID });
                    table.ForeignKey(
                        name: "FK_BusinessField_Business",
                        column: x => x.businessID,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessField_Field",
                        column: x => x.fieldID,
                        principalTable: "Field",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Investor",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    userID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    lastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    firstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    phoneNum = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDCard = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    dateOfBirth = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    taxIdentificationNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    bankAccount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    investorTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investor", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Investor_InvestorType",
                        column: x => x.investorTypeID,
                        principalTable: "InvestorType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    businessID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    roleID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.roleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessWallet",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    businessID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    balance = table.Column<double>(type: "float", nullable: true),
                    walletTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessWallet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BusinessWallet_Business",
                        column: x => x.businessID,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessWallet_WalletType",
                        column: x => x.walletTypeID,
                        principalTable: "WalletType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemWallet",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    balance = table.Column<double>(type: "float", nullable: true),
                    walletTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemWallet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SystemWallet_WalletType",
                        column: x => x.walletTypeID,
                        principalTable: "WalletType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    price = table.Column<double>(type: "float", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    minForPurchasing = table.Column<int>(type: "int", nullable: true),
                    maxForPurchasing = table.Column<int>(type: "int", nullable: true),
                    openDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    closeDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    approvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    approvedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Package_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodRevenue",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    periodNum = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodRevenue", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PeriodRevenue_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectHighlight",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHighlight", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProjectHighlight_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUpdate",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUpdate", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProjectUpdate_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Risk",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    riskTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risk", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Risk_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Risk_RiskType",
                        column: x => x.riskTypeID,
                        principalTable: "RiskType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stage",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    percents = table.Column<double>(type: "float", nullable: true),
                    openMonth = table.Column<int>(type: "int", nullable: true),
                    closeMonth = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Stage_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    createBy = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateBy = table.Column<DateTime>(type: "datetime", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Voucher_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestorLocation",
                columns: table => new
                {
                    investorID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    areaID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorLocation", x => new { x.investorID, x.areaID });
                    table.ForeignKey(
                        name: "FK_InvestorLocation_Area",
                        column: x => x.areaID,
                        principalTable: "Area",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorLocation_Investor",
                        column: x => x.investorID,
                        principalTable: "Investor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestorWallet",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    investorID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    balance = table.Column<double>(type: "float", nullable: true),
                    walletTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    upDateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorWallet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvestorWallet_Investor",
                        column: x => x.investorID,
                        principalTable: "Investor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorWallet_WalletType",
                        column: x => x.walletTypeID,
                        principalTable: "WalletType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Investment",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    investorID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    projectID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    packageID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    totalPrice = table.Column<double>(type: "float", nullable: true),
                    lastPayment = table.Column<DateTime>(type: "datetime", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Investment_Investor",
                        column: x => x.investorID,
                        principalTable: "Investor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Investment_Package",
                        column: x => x.packageID,
                        principalTable: "Package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Investment_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    periodRevenueID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    fromID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    toID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payment_PeriodRevenue",
                        column: x => x.periodRevenueID,
                        principalTable: "PeriodRevenue",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodRevenueHistory",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    periodRevenueID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodRevenueHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PeriodRevenueHistory_PeriodRevenue",
                        column: x => x.periodRevenueID,
                        principalTable: "PeriodRevenue",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageVoucher",
                columns: table => new
                {
                    packageID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    voucherID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    maxQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVoucher", x => new { x.packageID, x.voucherID });
                    table.ForeignKey(
                        name: "FK_PackageVoucher_Package",
                        column: x => x.packageID,
                        principalTable: "Package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageVoucher_Voucher",
                        column: x => x.voucherID,
                        principalTable: "Voucher",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoucherItem",
                columns: table => new
                {
                    investorID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    voucherId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    investmentID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    issuedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    expireDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    redeemDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    availableDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherItem", x => new { x.investorID, x.voucherId });
                    table.ForeignKey(
                        name: "FK_VoucherItem_Investor",
                        column: x => x.investorID,
                        principalTable: "Investor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoucherItem_Voucher",
                        column: x => x.voucherId,
                        principalTable: "Voucher",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    paymentID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    fromWalletID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    toWalletID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    fee = table.Column<double>(type: "float", nullable: true),
                    createDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    updateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updateBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    investmentID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transaction_Investment",
                        column: x => x.investmentID,
                        principalTable: "Investment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Payment",
                        column: x => x.paymentID,
                        principalTable: "Payment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessField_fieldID",
                table: "BusinessField",
                column: "fieldID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessWallet_businessID",
                table: "BusinessWallet",
                column: "businessID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessWallet_walletTypeID",
                table: "BusinessWallet",
                column: "walletTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_investorID",
                table: "Investment",
                column: "investorID");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_packageID",
                table: "Investment",
                column: "packageID");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_projectID",
                table: "Investment",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_Investor_investorTypeID",
                table: "Investor",
                column: "investorTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorLocation_areaID",
                table: "InvestorLocation",
                column: "areaID");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorWallet_investorID",
                table: "InvestorWallet",
                column: "investorID");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorWallet_walletTypeID",
                table: "InvestorWallet",
                column: "walletTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Package_projectID",
                table: "Package",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVoucher_voucherID",
                table: "PackageVoucher",
                column: "voucherID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_periodRevenueID",
                table: "Payment",
                column: "periodRevenueID");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodRevenue_projectID",
                table: "PeriodRevenue",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodRevenueHistory_periodRevenueID",
                table: "PeriodRevenueHistory",
                column: "periodRevenueID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_areaID",
                table: "Project",
                column: "areaID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_businessID",
                table: "Project",
                column: "businessID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHighlight_projectID",
                table: "ProjectHighlight",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUpdate_projectID",
                table: "ProjectUpdate",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_Risk_projectID",
                table: "Risk",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_Risk_riskTypeID",
                table: "Risk",
                column: "riskTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_projectID",
                table: "Stage",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_SystemWallet_walletTypeID",
                table: "SystemWallet",
                column: "walletTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_investmentID",
                table: "Transaction",
                column: "investmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_paymentID",
                table: "Transaction",
                column: "paymentID");

            migrationBuilder.CreateIndex(
                name: "IX_User_roleID",
                table: "User",
                column: "roleID");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_projectID",
                table: "Voucher",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItem_voucherId",
                table: "VoucherItem",
                column: "voucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessField");

            migrationBuilder.DropTable(
                name: "BusinessWallet");

            migrationBuilder.DropTable(
                name: "InvestorLocation");

            migrationBuilder.DropTable(
                name: "InvestorWallet");

            migrationBuilder.DropTable(
                name: "PackageVoucher");

            migrationBuilder.DropTable(
                name: "PeriodRevenueHistory");

            migrationBuilder.DropTable(
                name: "ProjectHighlight");

            migrationBuilder.DropTable(
                name: "ProjectUpdate");

            migrationBuilder.DropTable(
                name: "Risk");

            migrationBuilder.DropTable(
                name: "Stage");

            migrationBuilder.DropTable(
                name: "SystemWallet");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "VoucherItem");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "RiskType");

            migrationBuilder.DropTable(
                name: "WalletType");

            migrationBuilder.DropTable(
                name: "Investment");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "Investor");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropTable(
                name: "PeriodRevenue");

            migrationBuilder.DropTable(
                name: "InvestorType");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Business");
        }
    }
}
