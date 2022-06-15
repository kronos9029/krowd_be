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
    public class InvestorWalletRepository : BaseRepository, IInvestorWalletRepository
    {
        public InvestorWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateInvestorWallet(InvestorWallet investorWalletDTO)
        {
            try
            {
                var query = "INSERT INTO InvestorWallet ("
                    + "         InvestorId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investorWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteInvestorWalletById(Guid investorWalletId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorWalletId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestorWallet>> GetAllInvestorWallets(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     InvestorId, "
                    + "                     WalletTypeId ASC ) AS Num, "
                    + "             * "
                    + "         FROM InvestorWallet "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvestorId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<InvestorWallet>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM InvestorWallet WHERE IsDeleted = 0 ORDER BY InvestorId, WalletTypeId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<InvestorWallet>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<InvestorWallet> GetInvestorWalletById(Guid investorWalletId)
        {
            try
            {
                string query = "SELECT * FROM InvestorWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investorWalletId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorWallet>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorWallet(InvestorWallet investorWalletDTO, Guid investorWalletId)
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         InvestorId = @InvestorId, "
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", investorWalletDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", investorWalletId, DbType.Guid);

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

        //CLEAR DATA
        public async Task<int> ClearAllInvestorWalletData()
        {
            try
            {
                var query = "DELETE FROM InvestorWallet";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
