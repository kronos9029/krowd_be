using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.CommonRepos.Validation
{
    public class ValidationRepository : BaseRepository, IValidationRepository
    {
        public ValidationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<dynamic> CheckExistenceId(string tableName, Guid id)
        {
            try
            {
                string query = "SELECT * FROM " + tableName + " WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
