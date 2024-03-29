﻿using Dapper;
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

        //COUNT ALL
        public async Task<int> CountAllPeriodRevenueHistories(Guid projectId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var whereClause = " WHERE PR.ProjectId = @ProjectId ";
                parameters.Add("ProjectId", projectId, DbType.Guid);

                var query = "SELECT COUNT(*) FROM (SELECT PRH.* FROM PeriodRevenueHistory PRH JOIN PeriodRevenue PR ON PRH.PeriodRevenueId = PR.Id " + whereClause + ") AS X";
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT BY PERIOD REVENUE ID
        public async Task<int> CountPeriodRevenueHistoryByPeriodRevenueId(Guid periodRevenueId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM PeriodRevenueHistory WHERE PeriodRevenueId = @PeriodRevenueId";
                var parameters = new DynamicParameters();
                parameters.Add("PeriodRevenueId", periodRevenueId, DbType.Guid);
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
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
                    + "         @CreateBy )";

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
        public async Task<List<PeriodRevenueHistory>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize, Guid projectId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var whereClause = " WHERE PR.ProjectId = @ProjectId ";
                parameters.Add("ProjectId", projectId, DbType.Guid);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     PRH.CreateDate DESC ) AS Num, "
                    + "             PRH.* "
                    + "         FROM PeriodRevenueHistory PRH JOIN PeriodRevenue PR ON PRH.PeriodRevenueId = PR.Id "
                    +           whereClause
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
                    
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query, parameters)).ToList();
                }

                else
                {
                    var query = "SELECT PRH.* FROM PeriodRevenueHistory PRH JOIN PeriodRevenue PR ON PRH.PeriodRevenueId = PR.Id " + whereClause + " ORDER BY PRH.CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query, parameters)).ToList();
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
