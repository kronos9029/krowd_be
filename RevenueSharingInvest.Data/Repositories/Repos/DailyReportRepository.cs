using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class DailyReportRepository : BaseRepository, IDailyReportRepository
    {
        public DailyReportRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //COUNT ALL
        public async Task<int> CountAllDailyReports(string projectId, string stageId, string roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";
                var fromCondition = " FROM DailyReport DR JOIN Stage S ON DR.StageId = S.Id ";
                var projectIdCondition = " AND S.ProjectId = @ProjectId ";
                var stageIdCondition = " AND S.Id = @StageId ";

                whereCondition = whereCondition + projectIdCondition;
                parameters.Add("ProjectId", Guid.Parse(projectId), DbType.Guid);

                if (stageId != null)
                {
                    whereCondition = whereCondition + stageIdCondition;
                    parameters.Add("StageId", Guid.Parse(stageId), DbType.Guid);
                }

                whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);

                var query = "SELECT COUNT(*) FROM (SELECT DR.* " + fromCondition + whereCondition + ") AS X";

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //CREATE
        public async Task<int> CreateDailyReports(DailyReport dailyReport, int numOfReport)
        {
            try
            {
                var query = "INSERT INTO DailyReport (StageId, ReportDate, CreateDate,  CreateBy, Status ) VALUES ";

                for (int i = 1; i <= numOfReport; i++)
                {
                    if (i == numOfReport)
                        query += "(" + "'" + dailyReport.StageId + "'" + "," + "'" + dailyReport.ReportDate + "'" + "," + "'" + dailyReport.CreateDate + "'" + "," + "'" + dailyReport.CreateBy + "'" + "," + "'" + dailyReport.Status + "'" + ")";
                    else
                        query += "(" + "'" + dailyReport.StageId + "'" + "," + "'" + dailyReport.ReportDate + "'" + "," + "'" + dailyReport.CreateDate + "'" + "," + "'" + dailyReport.CreateBy + "'" + "," + "'" + dailyReport.Status + "'" + "), ";

                    dailyReport.ReportDate = dailyReport.ReportDate.AddDays(1);
                }
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<DailyReport>> GetAllDailyReports(int pageIndex, int pageSize, string projectId, string stageId, string roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";
                var fromCondition = " FROM DailyReport DR JOIN Stage S ON DR.StageId = S.Id ";
                var projectIdCondition = " AND S.ProjectId = @ProjectId ";
                var stageIdCondition = " AND S.Id = @StageId ";

                whereCondition = whereCondition + projectIdCondition;
                parameters.Add("ProjectId", Guid.Parse(projectId), DbType.Guid);

                if (stageId != null)
                {
                    whereCondition = whereCondition + stageIdCondition;
                    parameters.Add("StageId", Guid.Parse(stageId), DbType.Guid);
                }

                whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     ReportDate ASC ) AS Num, "
                    + "             DR.* "
                    +           fromCondition
                    +           whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         StageId, "
                    + "         Amount, "
                    + "         ReportDate, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";;
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<DailyReport>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT DR.* " + fromCondition + whereCondition + " ORDER BY ReportDate ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<DailyReport>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<DailyReport> GetDailyReportById(Guid id)
        {
            try
            {
                string query = "SELECT * "
                    + "         FROM "
                    + "             DailyReport "
                    + "         WHERE "
                    + "             Id = @Id ";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<DailyReport>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY PROJECT ID AND DATE
        public async Task<DailyReport> GetDailyReportByProjectIdAndDate(Guid projectId, string date)
        {
            try
            {
                string query = "SELECT DR.* "
                    + "         FROM " 
                    + "             DailyReport DR " 
                    + "             JOIN Stage S ON DR.StageId = S.Id "
                    + "         WHERE "
                    + "             S.ProjectId = @ProjectId "
                    + "             AND DR.ReportDate = @Date ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("Date", DateTime.ParseExact(date.Remove(date.Length - 8) + "00:00:00", "dd/MM/yyyy HH:mm:ss", null), DbType.DateTime);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<DailyReport>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateDailyReport(DailyReport dailyReport)
        {
            try
            {
                var query = "UPDATE DailyReport "
                    + "     SET "
                    + "         Amount = ISNULL(@Amount, Amount), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Amount", dailyReport.Amount, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", "Client", DbType.String);
                parameters.Add("Id", dailyReport.Id, DbType.Guid);

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
