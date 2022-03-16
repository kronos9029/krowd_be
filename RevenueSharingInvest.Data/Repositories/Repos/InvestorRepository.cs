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
    public class InvestorRepository : BaseRepository, IInvestorRepository
    {
        public InvestorRepository(IConfiguration configuration) : base(configuration)
        {
        }

<<<<<<< Updated upstream
=======
        public async Task<int> CreateInvestor(Investor investor)
        {
            try
            {
                var query = "INSERT INTO Investor (Id, UserId, CreateDate, InvestorTypeId) VALUES (@Id, @UserId, @CreateDate, @InvestorTypeId)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investor.Id, DbType.Guid);
                parameters.Add("UserId", investor.UserId, DbType.Guid);
                parameters.Add("CreateDate", investor.CreateDate, DbType.DateTime);
                parameters.Add("InvestorTypeId", investor.InvestorTypeId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> CreateInvestorType(InvestorType investorType)
        {
            try
            {
                var query = "INSERT INTO InvestorType (Name, Description, CreateDate, CreateBy) VALUES (@Name, @Description, @CreateDate, @CreateBy)";
                var parameters = new DynamicParameters();
                parameters.Add("Name", investorType.Name, DbType.String);
                parameters.Add("Description", investorType.Description, DbType.String);
                parameters.Add("CreateDate", investorType.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", investorType.CreateBy, DbType.String);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            } catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }

        }

        public async Task<List<InvestorType>> GetAllInvestorType()
        {
            try
            {
                string query = "SELECT * FROM InvestorType";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestorType>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
>>>>>>> Stashed changes
    }
}
