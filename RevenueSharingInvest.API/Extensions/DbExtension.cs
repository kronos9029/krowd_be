using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using StackExchange.Redis;

namespace RevenueSharingInvest.API.Extensions
{
    public static class DbExtension
    {

        public static void AddApplicationDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Read the connection string from appsettings.
            string dbConnectionString = configuration.GetConnectionString("PROD");

            // Inject IDbConnection, with implementation from SqlConnection class.
            services.AddTransient<IDbConnection>((sp) => new SqlConnection(dbConnectionString));

            //Register DBcontext for migration
            services.AddDbContext<Data.Helpers.KrowdContext>(options => options.UseSqlServer(dbConnectionString));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisCache");
            });
        }

    }
}
