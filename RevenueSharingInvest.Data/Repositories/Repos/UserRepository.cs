using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            string query = "SELECT * FROM User WHERE Email=@Email";
            var parameters = new DynamicParameters();
            parameters.Add("Email", userEmail, DbType.String);

            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
        }

        public async Task<int> CreateUser(string email, DateTime createDate, string imageUrl, string businessId, string investorId, string roleId)
        {
            var query = "INSERT INTO Customer (Email, CreateDate, Image, BusinessId, InvestorId, RoleId) VALUES (@Email, @CreateDate, @Image, @BusinessId, @InvestorId, @RoleId)";
            var parameters = new DynamicParameters();
            parameters.Add("Email", email, DbType.String);
            parameters.Add("CreateDate", createDate, DbType.DateTime);
            parameters.Add("BusinessId", businessId, DbType.String);
            parameters.Add("InvestorId", investorId, DbType.String);
            parameters.Add("RoleId", roleId, DbType.String);

            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
