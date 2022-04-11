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
    public class BusinessRepository : BaseRepository, IBusinessRepository
    {
        public BusinessRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateBusiness(Business newBusiness)
        {
            try
            {
                var query = "INSERT INTO Business (Id, Image, Email) VALUES (@Id, @Image, @Email)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", newBusiness.Id, DbType.Guid);
                parameters.Add("Image", newBusiness.Image, DbType.String);
                parameters.Add("Email", newBusiness.Email, DbType.String);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<Business>> GetAllBusiness()
        {
            try
            {
                string query = "SELECT * FROM Business";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Business>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public Task<Business> GetBusinessById(Guid businesssId)
        {
            throw new NotImplementedException();
        }
    }
}
