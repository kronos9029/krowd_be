using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class BillRepository : BaseRepository, IBillRepository
    {
        public BillRepository(IConfiguration configuration) : base(configuration)
        {
        }
        
        //CREATE
        public async Task<int> BulkInsertInvoice(InsertBillDTO request)
        {
            try
            {
                var query = "INSERT INTO Bill (InvoiceId, Amount, Description, CreateBy,  CreateDate, DailyReportId) VALUES ";

                for (int i = 0; i < request.bills.Count; i++)
                {
                    if (i == request.bills.Count - 1)
                        query += "('" + request.bills[i].invoiceId + "'" + "," + request.bills[i].amount  + "," + "'" + (request.bills[i].description ??= "") + "'" + "," + "'" + request.bills[i].createBy + "'" + "," + "'" + DateTime.ParseExact(request.bills[i].createDate, "dd/MM/yyyy HH:mm:ss", null) + "'" + "," + "'" + request.dailyReportId + "')";
                    else
                        query += "('" + request.bills[i].invoiceId + "'" + "," + request.bills[i].amount  + "," + "'" + (request.bills[i].description ??= "") + "'" + "," + "'" + request.bills[i].createBy + "'" + "," + "'" + DateTime.ParseExact(request.bills[i].createDate, "dd/MM/yyyy HH:mm:ss", null) + "'" + "," + "'" + request.dailyReportId + "'),";

                }
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //COUNT
        public async Task<int> CountAllBills(Guid dailyReportId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereClause = " WHERE DailyReportId = @DailyReportId ";
                parameters.Add("DailyReportId", dailyReportId, DbType.Guid);
                
                var query = "SELECT COUNT(*) FROM (SELECT * FROM Bill " + whereClause + " ) AS X";
                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));

            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //DELETE BY PROJECT ID
        public async Task<int> DeleteBillByProjectId(Guid projectId)
        {
            try
            {
                var query = "DELETE FROM Bill "
                    + "     WHERE "
                    + "         DailyReportId IN  "
                    + "         (SELECT "
                    + "             DR.Id "
                    + "         FROM "
                    + "             DailyReport DR "
                    + "             JOIN Stage S ON DR.StageId = S.Id "
                    + "         WHERE "
                    + "             S.ProjectId = @ProjectId)";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<int> DeleteBillsByDailyReportId(Guid dailyReportId)
        {
            try
            {
                var query = "DELETE FROM Bill "
                    + "     WHERE DailyReportId = @DailyReportId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("DailyReportId", dailyReportId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Bill>> GetAllBills(int pageIndex, int pageSize, Guid dailyReportId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereClause = " WHERE DailyReportId = @DailyReportId ";
                parameters.Add("DailyReportId", dailyReportId, DbType.Guid);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     CreateDate DESC ) AS Num, "
                    + "             * "
                    + "         FROM Bill "
                    +           whereClause
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvoiceId, "
                    + "         DailyReportId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         CreateBy, "
                    + "         CreateDate "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Bill>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Bill " + whereClause + " ORDER BY CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Bill>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Bill> GetBillById(Guid id)
        {
            try
            {
                string query = "SELECT * FROM Bill WHERE Id = @Id ";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Bill>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
