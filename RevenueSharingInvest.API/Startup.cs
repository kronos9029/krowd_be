using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevenueSharingInvest.API.Helpers;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.Helpers;
using RevenueSharingInvest.Data.Repositories.CommonRepos;
using RevenueSharingInvest.Data.Repositories.CommonRepos.Validation;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
       
        public void ConfigureServices(IServiceCollection services)
        {
            // Read the connection string from appsettings.
            string dbConnectionString = this.Configuration.GetConnectionString("DEV");

            // Inject IDbConnection, with implementation from SqlConnection class.
            services.AddTransient<IDbConnection>((sp) => new SqlConnection(dbConnectionString));

            //Register DBcontext for migration
           services.AddDbContext<KrowdContext>(options => options.UseSqlServer(dbConnectionString));

            //Authorize
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register your regular repositories
            // services.AddScoped<IDiameterRepository, DiameterRepository>();

            ////////// 
            ///

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
           
            // ADMIN SAFE LIST
            services.AddScoped<ClientIpCheckActionFilter>(container =>
            {
                var loggerFactory = container.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ClientIpCheckActionFilter>();

                return new ClientIpCheckActionFilter(
                    Configuration["AdminSafeList"], logger);
            });

            //CORS
            services.AddCors(options =>      
                options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            ));

            // Configure Strongly Typed Settings Objects
            ///Secret key
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            ///Firebase storage
            var firebaseSettingSection = Configuration.GetSection("FirebaseSettings");
            services.Configure<FirebaseSettings>(firebaseSettingSection);
            var firebaseSettings = firebaseSettingSection.Get<FirebaseSettings>(); 

            //Firebase authentication
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseSettings.ProjectId,
                ServiceAccountId = firebaseSettings.ServiceAccountId
            });

            services.AddControllers();

            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

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

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RevenueSharingInvest.API v1"));
            }

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseMiddleware<AuthorizeMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
