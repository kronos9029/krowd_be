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
    public class AccountTransactionRepository : BaseRepository, IAccountTransactionRepository
    {
        public AccountTransactionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateAccountTransaction(AccountTransaction accountTransactionDTO)
        {
            try
            {
                var query = "INSERT INTO AccountTransaction ("
                    + "         FromUserId, "
                    + "         ToUserId, "
                    + "         Description, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @FromUserId, "
                    + "         @ToUserId, "
                    + "         @Description, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("FromUserId", accountTransactionDTO.FromUserId, DbType.Guid);
                parameters.Add("ToUserId", accountTransactionDTO.ToUserId, DbType.Guid);
                parameters.Add("Description", accountTransactionDTO.Description, DbType.String);
                parameters.Add("Status", accountTransactionDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", accountTransactionDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", accountTransactionDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteAccountTransactionById(Guid accountTransactionId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE AccountTransaction "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", accountTransactionDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", accountTransactionId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<AccountTransaction>> GetAllAccountTransactions()
        {
            try
            {
                string query = "SELECT * FROM AccountTransaction WHERE IsDeleted = 0";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<AccountTransaction>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<AccountTransaction> GetAccountTransactionById(Guid accountTransactionId)
        {
            try
            {
                string query = "SELECT * FROM AccountTransaction WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", accountTransactionId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<AccountTransaction>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateAccountTransaction(AccountTransaction accountTransactionDTO, Guid accountTransactionId)
        {
            try
            {
                var query = "UPDATE AccountTransaction "
                    + "     SET "
                    + "         FromUserId = @FromUserId, "
                    + "         ToUserId = @ToUserId, "
                    + "         Description = @Description, "
                    + "         Status = @Status, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("FromUserId", accountTransactionDTO.FromUserId, DbType.Guid);
                parameters.Add("ToUserId", accountTransactionDTO.ToUserId, DbType.Guid);
                parameters.Add("Description", accountTransactionDTO.Description, DbType.String);
                parameters.Add("Status", accountTransactionDTO.Status, DbType.String);
                parameters.Add("CreateDate", accountTransactionDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", accountTransactionDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", accountTransactionDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", accountTransactionDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", accountTransactionId, DbType.Guid);

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
