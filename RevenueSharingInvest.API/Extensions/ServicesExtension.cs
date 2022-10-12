using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Repositories.ExtensionsRepos.Validation;
using RevenueSharingInvest.Data.Repositories.ExtensionsRepos;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using RevenueSharingInvest.Business.Helpers;
using static Dapper.SqlMapper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq;
using Google.Cloud.Firestore;
using RevenueSharingInvest.Data.Helpers;
using System.Text.Json;

namespace RevenueSharingInvest.API.Extensions
{
    public static class ConfigureAppServices
    {

        public static void AddConfigureAppServices(this IServiceCollection services, IConfiguration configuration) 
        {
            //Authorize
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //AUTHENTICATE
            services.AddScoped<IAuthenticateService, AuthenticateService>();

            //ACCOUNT TRANSACTION
            services.AddScoped<IAccountTransactionService, AccountTransactionService>();
            services.AddScoped<IAccountTransactionRepository, AccountTransactionRepository>();

            //AREA
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IAreaRepository, AreaRepository>();

            //BUSINESS
            services.AddScoped<IBusinessService, BusinessService>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();

            //BUSINESS FIELD
            services.AddScoped<IBusinessFieldService, BusinessFieldService>();
            services.AddScoped<IBusinessFieldRepository, BusinessFieldRepository>();

            //FIELD
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<IFieldRepository, FieldRepository>();

            //INVESTMENT
            services.AddScoped<IInvestmentService, InvestmentService>();
            services.AddScoped<IInvestmentRepository, InvestmentRepository>();

            //INVESTOR
            services.AddScoped<IInvestorRepository, InvestorRepository>();
            services.AddScoped<IInvestorService, InvestorService>();

            //INVESTOR TYPE
            services.AddScoped<IInvestorTypeRepository, InvestorTypeRepository>();
            services.AddScoped<IInvestorTypeService, InvestorTypeService>();

            //INVESTOR WALLET
            services.AddScoped<IInvestorWalletRepository, InvestorWalletRepository>();
            services.AddScoped<IInvestorWalletService, InvestorWalletService>();

            //PACKAGE
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPackageService, PackageService>();

            //PACKAGE VOUCHER
            services.AddScoped<IPackageVoucherRepository, PackageVoucherRepository>();
            services.AddScoped<IPackageVoucherService, PackageVoucherService>();

            //PAYMENT
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();

            //PERIOD REVENUE
            services.AddScoped<IPeriodRevenueRepository, PeriodRevenueRepository>();
            services.AddScoped<IPeriodRevenueService, PeriodRevenueService>();

            //PERIOD REVENUE HISTORY
            services.AddScoped<IPeriodRevenueHistoryRepository, PeriodRevenueHistoryRepository>();
            services.AddScoped<IPeriodRevenueHistoryService, PeriodRevenueHistoryService>();

            //PROJECT
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectService, ProjectService>();

            //PROJECT ENTITY
            services.AddScoped<IProjectEntityRepository, ProjectEntityRepository>();
            services.AddScoped<IProjectEntityService, ProjectEntityService>();

            //PROJECT WALLET
            services.AddScoped<IProjectWalletRepository, ProjectWalletRepository>();
            services.AddScoped<IProjectWalletService, ProjectWalletService>();

            //RISK
            services.AddScoped<IRiskRepository, RiskRepository>();
            services.AddScoped<IRiskService, RiskService>();

            //RISK TYPE
            services.AddScoped<IRiskTypeRepository, RiskTypeRepository>();
            services.AddScoped<IRiskTypeService, RiskTypeService>();

            //ROLE
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            //STAGE
            services.AddScoped<IStageRepository, StageRepository>();
            services.AddScoped<IStageService, StageService>();

            //SYSTEM WALLET
            services.AddScoped<ISystemWalletRepository, SystemWalletRepository>();
            services.AddScoped<ISystemWalletService, SystemWalletService>();

            //USER
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            //VOUCHER
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IVoucherService, VoucherService>();

            //VOUCHER ITEM
            services.AddScoped<IVoucherItemRepository, VoucherItemRepository>();
            services.AddScoped<IVoucherItemService, VoucherItemService>();

            // WALLET TRANSACTION
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();

            // WALLET TYPE
            services.AddScoped<IWalletTypeRepository, WalletTypeRepository>();
            services.AddScoped<IWalletTypeService, WalletTypeService>();

            //MOMO
            services.AddScoped<IMomoService, MomoService>();

            //////////   
            // VALIDATION
            services.AddScoped<IValidationRepository, ValidationRepository>();
            services.AddScoped<IValidationService, ValidationService>();

            // PROJECT TAG
            services.AddScoped<IProjectTagService, ProjectTagService>();
            //////////       
            ///
            //Upload File To Firebase
            services.AddScoped<IFileUploadService, FileUploadService>();
        }

        public static void AddSafeListAnnotation(this IServiceCollection services, IConfiguration configuration)
        {
            // ADMIN SAFE LIST
            services.AddScoped<ClientIpCheckActionFilter>(container =>
            {
                var loggerFactory = container.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ClientIpCheckActionFilter>();

                return new ClientIpCheckActionFilter(
                    configuration["AdminSafeList"], logger);
            });
        }

        public static void AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            ///Firebase storage
            var firebaseSettingSection = configuration.GetSection("FirebaseSettings");
            services.Configure<FirebaseSettings>(firebaseSettingSection);
            var firebaseSettings = firebaseSettingSection.Get<FirebaseSettings>();

            //Firebase authentication
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseSettings.ProjectId,
                ServiceAccountId = firebaseSettings.ServiceAccountId
            });
        }

        public static void AddFirestoreDatabasecontext(this IServiceCollection services, IConfiguration configuration)
        {
            var firestoreSettingSection = configuration.GetSection("FirestoreSettings");
            services.Configure<firestoreSettings>(firestoreSettingSection);
            var firestoreSettings = firestoreSettingSection.Get<firestoreSettings>();
            var firestoreJson = JsonSerializer.Serialize(firestoreSettings);
            services.AddSingleton(_ => new FirestoreProvider(
                new FirestoreDbBuilder
                {
                    ProjectId = firestoreSettings.ProjectId,
                    JsonCredentials = firestoreJson // <-- service account json file
                }.Build()
            ));
        }

        public static void AddSettingObjects(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Strongly Typed Settings Objects
            ///Secret key
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            ///Momo Transaction
            var momoSettingsSection = configuration.GetSection("MomoSettings");
            services.Configure<MomoSettings>(momoSettingsSection);
            var momoSettings = momoSettingsSection.Get<MomoSettings>();
        }

        public static void AddJWTServices(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            //JWT
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public static void AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RevenueSharingInvest.API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer YOUR_TOKEN_HERE\"",

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                           new OpenApiSecurityScheme
                             {
                                 Reference = new OpenApiReference
                                 {
                                     Type = ReferenceType.SecurityScheme,
                                     Id = "Bearer"
                                 }
                             },
                             new string[] {}
                     }
                 });
            });
        }




    }
}
