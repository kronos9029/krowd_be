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
        public async Task<string> CreateAccountTransaction(AccountTransaction accountTransactionDTO)
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
                    + "         IsDeleted, "
                    + "         PartnerCode, "
                    + "         OrderId, "
                    + "         RequestId, "
                    + "         Amount, "
                    + "         ResponseTime, "
                    + "         Message, "
                    + "         ResultCode, "
                    + "         PayUrl, "
                    + "         Deeplink, "
                    + "         QrCodeUrl ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @FromUserId, "
                    + "         @ToUserId, "
                    + "         @Description, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0, "
                    + "         @PartnerCode, "
                    + "         @OrderId, "
                    + "         @RequestId, "
                    + "         @Amount, "
                    + "         @ResponseTime, "
                    + "         @Message, "
                    + "         @ResultCode, "
                    + "         @PayUrl, "
                    + "         @Deeplink, "
                    + "         @QrCodeUrl ) ";

                var parameters = new DynamicParameters();
                parameters.Add("FromUserId", accountTransactionDTO.FromUserId, DbType.Guid);
                parameters.Add("ToUserId", accountTransactionDTO.ToUserId, DbType.Guid);
                parameters.Add("Description", accountTransactionDTO.Description, DbType.String);
                parameters.Add("Status", accountTransactionDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", accountTransactionDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", accountTransactionDTO.UpdateBy, DbType.Guid);
                parameters.Add("PartnerCode", accountTransactionDTO.PartnerCode, DbType.String);
                parameters.Add("OrderId", accountTransactionDTO.OrderId, DbType.String);
                parameters.Add("RequestId", accountTransactionDTO.RequestId, DbType.String);
                parameters.Add("Amount", accountTransactionDTO.Amount, DbType.Int32);
                parameters.Add("ResponseTime", accountTransactionDTO.ResponseTime, DbType.Int32);
                parameters.Add("Message", accountTransactionDTO.Message, DbType.String);
                parameters.Add("ResultCode", accountTransactionDTO.ResultCode, DbType.String);
                parameters.Add("PayUrl", accountTransactionDTO.PayUrl, DbType.String);
                parameters.Add("Deeplink", accountTransactionDTO.Deeplink, DbType.String);
                parameters.Add("QrCodeUrl", accountTransactionDTO.QrCodeUrl, DbType.String);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
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
        public async Task<List<AccountTransaction>> GetAllAccountTransactions(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     Status ASC ) AS Num, "
                    + "             * "
                    + "         FROM AccountTransaction "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         FromUserId, "
                    + "         ToUserId, "
                    + "         Description, "
                    + "         Status, "
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
                    return (await connection.QueryAsync<AccountTransaction>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM AccountTransaction WHERE IsDeleted = 0 ORDER BY Status ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<AccountTransaction>(query)).ToList();
                }              
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

        //CLEAR DATA
        public async Task<int> ClearAllAccountTransactionData()
        {
            try
            {
                var query = "DELETE FROM AccountTransaction";
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
