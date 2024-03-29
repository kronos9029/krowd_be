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
    public class AreaRepository : BaseRepository, IAreaRepository
    {
        public AreaRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateArea(Area areaDTO)
        {
            try
            {
                var query = "INSERT INTO Area ("
                    + "         City, " 
                    + "         District, " 
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @City, "
                    + "         @District, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("City", areaDTO.City, DbType.String);
                parameters.Add("District", areaDTO.District, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", areaDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid) connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteAreaById(Guid areaId)//thiếu para UpdateBy
        {
            try
            {
                var query = "DELETE FROM Area WHERE Id = @Id ";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", areaId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Area>> GetAllAreas(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     City ASC ) AS Num, "
                    + "             * "
                    + "         FROM Area ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         City, "
                    + "         District, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Area>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Area ORDER BY City ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Area>(query)).ToList();
                }             
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Area> GetAreaById(Guid areaId)
        {
            try
            {
                string query = "SELECT * FROM Area WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", areaId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Area>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateArea(Area areaDTO, Guid areaId)
        {
            try
            {
                var query = "UPDATE Area "
                    + "     SET "
                    + "         City = ISNULL(@City, City), "
                    + "         District = ISNULL(@District, District),"
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("City", areaDTO.City, DbType.String);
                parameters.Add("District", areaDTO.District, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", areaId, DbType.Guid);

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

        //CLEAR DATA
        public async Task<int> ClearAllAreaData()
        {
            try
            {
                var query = "DELETE FROM Area";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<Area> GetAreaByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT "
                    + "         A.* "
                    + "     FROM "
                    + "         Area A "
                    + "         JOIN Project P ON A.Id = P.AreaId "
                    + "     WHERE "
                    + "         P.Id = @ProjectId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Area>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
