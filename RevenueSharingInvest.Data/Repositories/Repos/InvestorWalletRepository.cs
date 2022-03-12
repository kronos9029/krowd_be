using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class InvestorWalletRepository : BaseRepository, IInvestorWalletRepository
    {
        public InvestorWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<float> GetInvestorTotalBalance(string ID)
        {
            try
            {
                var query = "SELECT SUM(balance) AS Balance "
                          + "FROM InvestorWallet "
                          + "WHERE investorID = @ID and isDeleted = 0 ";
                var parameters = new DynamicParameters();
                parameters.Add("ID", ID, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<float>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
