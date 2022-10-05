using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
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
    public class RiskTypeRepository : BaseRepository, IRiskTypeRepository
    {
        public RiskTypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateRiskType(RiskType riskTypeDTO)
        {
            try
            {
                var query = "INSERT INTO RiskType ("
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
                parameters.Add("Name", riskTypeDTO.Name, DbType.String);
                parameters.Add("Description", riskTypeDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", riskTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", riskTypeDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteRiskTypeById(Guid riskTypeId)
        {
            try
            {
                var query = "DELETE FROM RiskType WHERE Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskTypeId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RiskType>> GetAllRiskTypes(int pageIndex, int pageSize)
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
                    + "         FROM RiskType "
                    + "         ) "
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
                    return (await connection.QueryAsync<RiskType>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM RiskType ORDER BY Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<RiskType>(query)).ToList();
                }                
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<RiskType> GetRiskTypeById(Guid riskTypeId)
        {
            try
            {
                string query = "SELECT * FROM RiskType WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RiskType>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateRiskType(RiskType riskTypeDTO, Guid riskTypeId)
        {
            try
            {
                var query = "UPDATE RiskType "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Description = @Description, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", riskTypeDTO.Name, DbType.String);
                parameters.Add("Description", riskTypeDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", riskTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", riskTypeId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<RiskTypeObject>> GetRiskTypeInUse(Guid riskTypeId)
        {
            try
            {
                var query = "SELECT rt.Id as RiskTypeId, r.Id as RiskId, r.ProjectId " +
                            "FROM RiskType rt JOIN Risk r ON rt.Id = r.RiskTypeId " +
                            "WHERE r.ProjectId IN (SELECT Id FROM Project) AND rt.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<RiskTypeObject>(query, parameters)).ToList();
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        } 
    }

    public class RiskTypeObject
    {
        public string RiskTypeId { get; set; }
        public string RiskId { get; set; }
        public string ProjectId { get; set; }
    }
}
