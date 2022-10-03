using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class WalletTransactionRepository : BaseRepository, IWalletTransactionRepository
    {
        public WalletTransactionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateWalletTransaction(WalletTransaction walletTransactionDTO)
        {
            try
            {
                var query = "INSERT INTO WalletTransaction ("
                    + "         PaymentId, "
                    + "         SystemWalletId, "
                    + "         ProjectWalletId, "
                    + "         InvestorWalletId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         Type, "
                    + "         FromWalletId, "
                    + "         ToWalletId, "
                    + "         Fee, "
                    + "         CreateDate, "
                    + "         CreateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @PaymentId, "
                    + "         @SystemWalletId, "
                    + "         @ProjectWalletId, "
                    + "         @InvestorWalletId, "
                    + "         @Amount, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @FromWalletId, "
                    + "         @ToWalletId, "
                    + "         @Fee, "
                    + "         @CreateDate, "
                    + "         @CreateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("PaymentId", walletTransactionDTO.PaymentId, DbType.Guid);
                parameters.Add("SystemWalletId", walletTransactionDTO.SystemWalletId, DbType.Guid);
                parameters.Add("ProjectWalletId", walletTransactionDTO.ProjectWalletId, DbType.Guid);
                parameters.Add("InvestorWalletId", walletTransactionDTO.InvestorWalletId, DbType.Guid);
                parameters.Add("Amount", walletTransactionDTO.Amount, DbType.Double);
                parameters.Add("Description", walletTransactionDTO.Description, DbType.String);
                parameters.Add("Type", walletTransactionDTO.Type, DbType.String);
                parameters.Add("FromWalletId", walletTransactionDTO.FromWalletId, DbType.Guid);
                parameters.Add("ToWalletId", walletTransactionDTO.ToWalletId, DbType.Guid);
                parameters.Add("Fee", walletTransactionDTO.Fee, DbType.Double);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", walletTransactionDTO.CreateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<WalletTransaction>> GetAllWalletTransactions(int pageIndex, int pageSize, Guid? userId, Guid? userRoleId, string fromDate, string toDate, string type, string order)
        {
            string selectColumns = " W.Id , W.PaymentId , W.SystemWalletId , W.ProjectWalletId , W.InvestorWalletId , W.Amount , W.Description , W.Type , W.FromWalletId , W.ToWalletId , W.Fee , W.CreateDate , W.CreateBy ";
            string whereClause = "";
            string fromClause = " FROM WalletTransaction W ";
            string orderClause = " ORDER BY W.CreateDate DESC ";

            string userIdCondition = " AND U.Id = @UserId ";
            string dateCondition = " AND W.CreateDate BETWEEN @FromDate AND @ToDate ";
            string typeCondition = " AND W.Type = @Type ";

            try
            {
                var parameters = new DynamicParameters();
                //if(!userId.Equals("")){

                //    whereClause = "WHERE CreateBy = '"+Guid.Parse(userId)+"'";
                //    if(!fromDate.Equals("") || !toDate.Equals(""))
                //    {
                //        whereClause += " AND CreateDate >= '"+fromDate+"' AND CreateDate <= '"+toDate+"'";
                //    }
                //    if (!walletId.Equals(""))
                //    {
                //        whereClause += " AND (FromWalletId = '" + Guid.Parse(walletId) + "' OR ToWalletId = '" + Guid.Parse(walletId) + "')";
                //    }
                //} else 
                //{
                //    if (!fromDate.Equals("") || !toDate.Equals(""))
                //    {
                //        whereClause += " WHERE CreateDate >= '" + fromDate + "' AND CreateDate <= '" + toDate + "'";
                //    }
                //    if (!walletId.Equals(""))
                //    {
                //        whereClause += " AND (FromWalletId = '" + Guid.Parse(walletId) + "' OR ToWalletId = '" + Guid.Parse(walletId) + "')";
                //    }
                //}

                //sort ??= "";
                //if (sort.Equals(""))
                //{
                //    sort = "DESC";
                //} else
                //{
                //    sort = "ASC";
                //}

                if (userRoleId != null)
                {
                    if (userRoleId.Equals(Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))))
                    {
                        fromClause = " FROM WalletTransaction W "
                            + "         JOIN ProjectWallet PW ON (W.FromWalletId = PW.Id OR W.ToWalletId = PW.Id) "
                            + "         JOIN [User] U ON PW.ProjectManagerId = U.Id ";
                    }
                    else if (userRoleId.Equals(Guid.Parse(RoleDictionary.role.GetValueOrDefault("INVESTOR"))))
                    {
                        fromClause = " FROM WalletTransaction W " 
                            + "         JOIN InvestorWallet IW ON (W.FromWalletId = IW.Id OR W.ToWalletId = IW.Id) " 
                            + "         JOIN Investor I ON IW.InvestorId = I.Id " 
                            + "         JOIN [User] U ON I.UserId = U.Id ";
                    }

                    whereClause = whereClause + userIdCondition;
                    parameters.Add("UserId", userId, DbType.Guid);
                }

                if (fromDate != null && toDate != null)
                {
                    whereClause = whereClause + dateCondition;
                    parameters.Add("FromDate", DateTime.ParseExact(fromDate, "dd/MM/yyyy HH:mm:ss", null), DbType.DateTime);
                    parameters.Add("ToDate", DateTime.ParseExact(toDate, "dd/MM/yyyy HH:mm:ss", null), DbType.DateTime);
                }

                if (type != null)
                {
                    whereClause = whereClause + typeCondition;
                    parameters.Add("Type", type, DbType.String);
                }

                if (order != null)
                {
                    orderClause = " ORDER BY W.CreateDate " + order;
                }

                whereClause = whereClause.Length != 0 ? "WHERE " + whereClause.Substring(4, whereClause.Length - 4) : "";

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    +               orderClause
                    + "             ) AS Num, "
                    +               selectColumns
                    +           fromClause
                    +           whereClause
                    + "         GROUP BY "
                    +               selectColumns
                    + "         )"
                    + "     SELECT  "
                    + "         Id, "
                    + "         PaymentId, "
                    + "         SystemWalletId, "
                    + "         ProjectWalletId, "
                    + "         InvestorWalletId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         Type, "
                    + "         FromWalletId, "
                    + "         ToWalletId, "
                    + "         Fee, "
                    + "         CreateDate, "
                    + "         CreateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WalletTransaction>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT DISTINCT W.* " + fromClause + whereClause + orderClause;
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WalletTransaction>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<WalletTransaction> GetWalletTransactionById(Guid walletTransactionId)
        {
            try
            {
                string query = "SELECT * FROM WalletTransaction WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", walletTransactionId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<WalletTransaction>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletTransaction(WalletTransaction walletTransactionDTO, Guid walletTransactionId)
        {
            try
            {
                var query = "UPDATE WalletTransaction "
                    + "     SET "
                    + "         PaymentId = @PaymentId, "
                    + "         SystemWalletId = @SystemWalletId, "
                    + "         ProjectWalletId = @ProjectWalletId, "
                    + "         InvestorWalletId = @InvestorWalletId, "
                    + "         Amount = @Amount, "
                    + "         Description = @Description, "
                    + "         Type = @Type, "
                    + "         FromWalletId = @FromWalletId, "
                    + "         ToWalletId = @ToWalletId, "
                    + "         Fee = @Fee"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("PaymentId", walletTransactionDTO.PaymentId, DbType.Guid);
                parameters.Add("SystemWalletId", walletTransactionDTO.SystemWalletId, DbType.Guid);
                parameters.Add("ProjectWalletId", walletTransactionDTO.ProjectWalletId, DbType.Guid);
                parameters.Add("InvestorWalletId", walletTransactionDTO.InvestorWalletId, DbType.Guid);
                parameters.Add("Amount", walletTransactionDTO.Amount, DbType.Double);
                parameters.Add("Description", walletTransactionDTO.Description, DbType.String);
                parameters.Add("Type", walletTransactionDTO.Type, DbType.String);
                parameters.Add("FromWalletId", walletTransactionDTO.FromWalletId, DbType.Guid);
                parameters.Add("ToWalletId", walletTransactionDTO.ToWalletId, DbType.Guid);
                parameters.Add("Fee", walletTransactionDTO.Fee, DbType.Double);
                parameters.Add("Id", walletTransactionId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
