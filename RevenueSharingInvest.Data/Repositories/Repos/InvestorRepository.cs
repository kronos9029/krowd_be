using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.DTOs;
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
    public class InvestorRepository : BaseRepository, IInvestorRepository
    {
        public InvestorRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Investor> GetInvestorByID(string ID)
        {
            try
            {
                var query = "SELECT * FROM Investor WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", ID, DbType.String);

                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Investor>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<InvestorDTO> GetProjectCEO(string projectID)
        {
            try
            {
                var query = "SELECT * FROM Investor WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectID, DbType.String);

                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorDTO>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
