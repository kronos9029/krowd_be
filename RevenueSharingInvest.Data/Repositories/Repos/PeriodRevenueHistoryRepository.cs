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
    public class PeriodRevenueHistoryRepository : BaseRepository, IPeriodRevenueHistoryRepository
    {
        public PeriodRevenueHistoryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO)
        {
            try
            {
                var query = "INSERT INTO PeriodRevenueHistory ("
                    + "         Name, "
                    + "         PeriodRevenueId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @PeriodRevenueId, "
                    + "         @Amount, "
                    + "         @Description, "
                    + "         @CreateDate, "
                    + "         @CreateB )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", periodRevenueHistoryDTO.Name, DbType.String);
                parameters.Add("PeriodRevenueId", periodRevenueHistoryDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Amount", periodRevenueHistoryDTO.Amount, DbType.Double);
                parameters.Add("Description", periodRevenueHistoryDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueHistoryDTO.CreateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePeriodRevenueHistoryByProjectId(Guid projectIdId)
        {
            try
            {
                var query = "DELETE FROM PeriodRevenueHistory "
                    + "     WHERE "
                    + "         Id IN  "
                    + "         (SELECT " 
                    + "             PH.Id " 
                    + "         FROM " 
                    + "             PeriodRevenueHistory PH " 
                    + "             JOIN PeriodRevenue P ON PH.PeriodRevenueId = P.Id " 
                    + "         WHERE " 
                    + "             P.ProjectId = @ProjectId)";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectIdId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueHistory>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     PeriodRevenueId, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM PeriodRevenueHistory "
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         PeriodRevenueId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query, parameters)).ToList();
                }

                else
                {
                    var query = "SELECT * FROM PeriodRevenueHistory ORDER BY PeriodRevenueId, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query)).ToList();
                }              
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<PeriodRevenueHistory> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        {
            try
            {
                string query = "SELECT * FROM PeriodRevenueHistory WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", periodRevenueHistoryId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<PeriodRevenueHistory>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO, Guid periodRevenueHistoryId)
        {
            try
            {
                var query = "UPDATE PeriodRevenueHistory "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         PeriodRevenueId = @PeriodRevenueId, "
                    + "         Description = @Description "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", periodRevenueHistoryDTO.Name, DbType.String);
                parameters.Add("PeriodRevenueId", periodRevenueHistoryDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Description", periodRevenueHistoryDTO.Description, DbType.String);
                parameters.Add("Id", periodRevenueHistoryId, DbType.Guid);

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
