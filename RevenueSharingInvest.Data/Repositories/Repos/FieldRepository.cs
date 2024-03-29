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
    public class FieldRepository : BaseRepository, IFieldRepository
    {
        public FieldRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateField(Field fieldDTO)
        {
            try
            {
                var query = "INSERT INTO Field ("
                    + "         Name, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", fieldDTO.Name, DbType.String);
                parameters.Add("Description", fieldDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", fieldDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", fieldDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteFieldById(Guid fieldId)
        {
            try
            {
                var query = " DELETE FROM Field WHERE Id = @Id ";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", fieldId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Field>> GetAllFields(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Field "
                    + "          ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         Description, "
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
                    return (await connection.QueryAsync<Field>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Field ORDER BY Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Field>(query)).ToList();
                }             
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT
        public int CountAllField()
        {
            try
            {
                var query = "SELECT COUNT(*) FROM Field";
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query);

            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<Field> GetFieldById(Guid fieldId)
        {
            try
            {
                string query = "SELECT * FROM Field WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", fieldId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Field>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET PROJECT BY FIELD ???
        public async Task<List<string>> GetProjectsByFieldId(Guid fieldId)
        {
            try
            {
                var query = "SELECT p.Name FROM Project p JOIN Field f ON p.FieldId = f.Id WHERE f.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", fieldId, DbType.Guid);
                using var connections = CreateConnection();
                return (await connections.QueryAsync<string>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY BUSINESS ID
        public async Task<List<Field>> GetFieldsByBusinessId(Guid businessId)
        {
            var query = "SELECT f.Id, f.Name, f.Description, f.CreateDate, f.CreateBy, f.UpdateDate, f.UpdateBy " +
                "FROM BusinessField bf JOIN Field f ON bf.FieldId = f.Id " +
                "WHERE bf.BusinessId = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", businessId, DbType.Guid);
            using var connections = CreateConnection();
            return (await connections.QueryAsync<Field>(query, parameters)).ToList();

        }

        //UPDATE
        public async Task<int> UpdateField(Field fieldDTO, Guid fieldId)
        {
            try
            {
                var query = "UPDATE Field "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Description = @Description, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", fieldDTO.Name, DbType.String);
                parameters.Add("Description", fieldDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", fieldDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", fieldId, DbType.Guid);

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

        //GET BY BUSINESS ID
        public async Task<List<Field>> GetCompanyFields(Guid businessId)
        {
            try
            {
                var query = "SELECT " 
                    + "         F.* " 
                    + "     FROM " 
                    + "         Field F " 
                    + "         JOIN BusinessField BF ON F.Id = BF.FieldId " 
                    + "     WHERE " 
                    + "         BF.BusinessId = @BusinessId "
                    + "     ORDER BY F.Name ASC";
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Field>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY PROJECT ID
        public async Task<Field> GetProjectFieldByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT "
                    + "         F.* "
                    + "     FROM "
                    + "         Field F "
                    + "         JOIN Project P ON F.Id = P.FieldId "
                    + "     WHERE "
                    + "         P.Id = @ProjectId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Field>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
