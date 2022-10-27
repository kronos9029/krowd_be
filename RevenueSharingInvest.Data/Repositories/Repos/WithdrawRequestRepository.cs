using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    public class WithdrawRequestRepository : BaseRepository, IWithdrawRequestRepository
    {
        public WithdrawRequestRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<string> CreateWithdrawRequest(WithdrawRequest request)
        {
            try
            {
                var query = "INSERT INTO WithdrawRequest ("
                            + "         BankName, "
                            + "         AccountName, "
                            + "         BankAccount, "
                            + "         Description, "
                            + "         Amount, "
                            + "         Status, "
                            + "         RefusalReason, "
                            + "         CreateDate, "
                            + "         CreateBy, "
                            + "         UpdateDate, "
                            + "         UpdateBy ) "
                            + "     OUTPUT "
                            + "         INSERTED.Id "
                            + "     VALUES ( "
                            + "         @BankName, "
                            + "         @AccountName, "
                            + "         @BankAccount, "
                            + "         @Description, "
                            + "         @Amount, "
                            + "         @Status, "
                            + "         @RefusalReason, "
                            + "         @CreateDate, "
                            + "         @CreateBy, "
                            + "         @UpdateDate, "
                            + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("BankName", request.BankName, DbType.String);
                parameters.Add("AccountName", request.AccountName, DbType.String);
                parameters.Add("BankAccount", request.BankAccount, DbType.String);
                parameters.Add("Description", request.Description, DbType.String);
                parameters.Add("Amount", request.Amount, DbType.Int64);
                parameters.Add("Status", request.Status, DbType.String);
                parameters.Add("RefusalReason", request.RefusalReason, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", request.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", request.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> AdminApproveWithdrawRequest(Guid userId, Guid requestId, string receipt)
        {
            try
            {
                var query = "UPDATE WithdrawRequest SET " +
                            "Status = '"+WithdrawRequestEnum.PARTIAL.ToString()+"', " +
                            "Description = ISNULL(@Description, Description), " +
                            "UpdateBy = @UpdateBy, " +
                            "UpdateDate = @UpdateDate " +
                            "WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("UpdateBy", userId, DbType.Guid);
                parameters.Add("Id", requestId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("Description", receipt, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }   
        }

        public async Task<int> InvestorApproveWithdrawRequest(Guid userId, Guid requestId)
        {
            try
            {
                var query = "UPDATE WithdrawRequest SET " +
                            "Status = '"+WithdrawRequestEnum.APPROVED.ToString()+"', " +
                            "UpdateBy = @UpdateBy, " +
                            "UpdateDate = @UpdateDate " +
                            "WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("UpdateBy", userId, DbType.Guid);
                parameters.Add("Id", requestId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }   
        }

        public async Task<int> AdminRejectWithdrawRequest(Guid userId, Guid requestId, string RefusalReason)
        {
            try
            {
                var query = "UPDATE WithdrawRequest SET " +
                            "Status = '"+WithdrawRequestEnum.REJECTED.ToString()+"', " +
                            "RefusalReason = @RefusalReason," +
                            "UpdateBy = @UpdateBy, " +
                            "UpdateDate = @UpdateDate " +
                            "WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("UpdateBy", userId, DbType.Guid);
                parameters.Add("Id", requestId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("RefusalReason", RefusalReason, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }   
        }
        

        public async Task<int> ReportWithdrawRequest(Guid userId, Guid requestId, string reportMessage)
        {
            try
            {
                var query = "UPDATE WithdrawRequest SET " +
                            "Status = '"+WithdrawRequestEnum.PARTIAL_ADMIN.ToString()+"', " +
                            "ReportMessage = @ReportMessage" +
                            "UpdateBy = @UpdateBy, " +
                            "UpdateDate = @UpdateDate " +
                            "WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("UpdateBy", userId, DbType.Guid);
                parameters.Add("Id", requestId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("ReportMessage", reportMessage, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }   
        }

        public async Task<WithdrawRequest> GetWithdrawRequestByRequestIdAndUserId(Guid requestId, Guid userId)
        {
            try
            {
                var query = "SELECT * FROM WithdrawRequest WHERE Id = @Id AND CreateBy = @CreateBy";
                var parameters = new DynamicParameters();
                parameters.Add("Id", requestId, DbType.Guid);
                parameters.Add("CreateBy", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<WithdrawRequest>(query, parameters);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<WithdrawRequest>> GetWithdrawRequestByUserId(Guid userId)
        {
            try
            {
                var query = "SELECT * FROM WithdrawRequest WHERE CreateBy = @CreateBy";
                var parameters = new DynamicParameters();
                parameters.Add("CreateBy", userId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WithdrawRequest>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<WithdrawRequest>> AdminGetAllWithdrawRequest()
        {
            try
            {
                var query = "SELECT * FROM WithdrawRequest";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WithdrawRequest>(query)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<WithdrawRequest>> GetAllWithdrawRequest(int pageIndex, int pageSize, string userId, string filter)
        {
            try
            {
                var parameters = new DynamicParameters();
                var whereClause = "";
                if (!userId.Equals(""))
                {
                    whereClause = " WHERE CreateBy = @UserId ";
                    parameters.Add("UserId", Guid.Parse(userId), DbType.Guid);
                }
                if (!filter.Equals("ALL"))
                {
                    if (whereClause.Equals(""))
                        whereClause = " WHERE Status = @Status ";
                    else
                        whereClause += " AND Status = @Status ";

                    parameters.Add("Status", filter, DbType.String);
                }

                
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     CreateDate DESC ) AS Num, "
                    + "             * "
                    + "         FROM  WithdrawRequest "
                    +           whereClause
                    + "         ) "
                    + "     SELECT *"
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";

                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WithdrawRequest>(query, parameters)).ToList();
                } else
                {
                    var query = "SELECT * FROM WithdrawRequest " + whereClause + " ORDER BY CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WithdrawRequest>(query, parameters)).ToList();
                }


            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }


        public async Task<WithdrawRequest> GetWithdrawRequestByRequestId(Guid requestId)
        {
            try
            {
                var query = "SELECT * FROM WithdrawRequest WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", requestId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<WithdrawRequest>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
