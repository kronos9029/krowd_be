using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.API.Hangfire;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

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
            services.AddApplicationDatabaseContext(Configuration);

            services.AddConfigureAppServices(Configuration);

            services.AddSafeListAnnotation(Configuration);

            //CORS
            services.AddCors(options =>
                options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            ));

            services.AddSettingObjects(Configuration);

            services.AddFirebaseAuthentication(Configuration);

            services.AddFirestoreDatabasecontext(Configuration);

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireDatabase")));

            services.AddHangfireServer();

            services.AddControllers();

            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            services.AddJWTServices(Configuration);

            services.AddSwaggerServices(Configuration);

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
                app.UseHangfireDashboard("/hangfire", options: new DashboardOptions
                {
                    Authorization = new List<IDashboardAuthorizationFilter>() { new HangfireAuthorizationFilter() },
                    IsReadOnlyFunc = context => true // according to your needs
                });
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
                endpoints.MapHangfireDashboard();
            });


        }
    }
}
