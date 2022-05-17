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
        public async Task<int> CreateInvestorWallet(InvestorWallet investorWalletDTO)
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
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         @Balance, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("Balance", investorWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investorWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
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
        public async Task<List<InvestorWallet>> GetAllInvestorWallets()
        {
            try
            {
                string query = "SELECT * FROM InvestorWallet";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestorWallet>(query)).ToList();
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
                    + "         Balance = @Balance, "
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("Balance", investorWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", investorWalletDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", investorWalletDTO.CreateBy, DbType.Guid);
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
    }
}
