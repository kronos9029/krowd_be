using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    public class StageRepository : BaseRepository, IStageRepository
    {
        public StageRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateStage(Stage stageDTO)
        {
            try
            {
                var query = "INSERT INTO Stage ("
                    + "         Name, "
                    + "         ProjectId, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @ProjectId, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("ProjectId", stageDTO.ProjectId, DbType.Guid);
                parameters.Add("StartDate", stageDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", stageDTO.EndDate, DbType.DateTime);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", stageDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);

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
        public async Task<List<Stage>> GetAllStagesByProjectId(Guid projectId, int pageIndex, int pageSize, string? status)
        {
            try
            {
                var parameters = new DynamicParameters();

                string whereClause = " WHERE ProjectId = @ProjectId ";

                if (status != null)
                {
                    whereClause = whereClause + " AND Status = @Status ";
                    parameters.Add("Status", status, DbType.String);
                }

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     CreateDate ASC ) AS Num, "
                    + "             * "
                    + "         FROM Stage "
                    +           whereClause
                    + "          ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         ProjectId, "
                    + "         Description, "
                    + "         Status, "
                    + "         IsOverDue, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Stage>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Stage " + whereClause + " ORDER BY CreateDate ASC";
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Stage>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Stage> GetStageById(Guid stageId)
        {
            try
            {
                string query = "SELECT * FROM Stage WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", stageId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Stage>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateStage(Stage stageDTO, Guid stageId)
        {
            try
            {
                var query = "UPDATE Stage "
                    + "     SET "
                    + "         Name = ISNULL(@Name, Name), "
                    + "         Description = ISNULL(@Description, Description), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("Description", stageDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", stageId, DbType.Guid);

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

        //DELETE ALL BY PROJECT ID
        public async Task<int> DeleteStageByProjectId(Guid projectId)
        {
            try
            {
                var query = "DELETE FROM Stage WHERE ProjectId = @ProjectId ";
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

        //GET BY PROJECT ID AND DATE
        public async Task<Stage> GetStageByProjectIdAndDate(Guid projectId, string date)
        {
            try
            {
                string query = "SELECT * " 
                    + "         FROM Stage " 
                    + "         WHERE " 
                    + "             ProjectId = @ProjectId " 
                    + "             AND @Date BETWEEN StartDate AND EndDate ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("Date", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Stage>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<Stage> GetLastStageByProjectId(Guid projectId)
        {
            try
            {
                string query = "SELECT "
                    + "             S.* "
                    + "         FROM "
                    + "             Stage S "
                    + "             JOIN ( "
                    + "                 SELECT MAX(EndDate) 'EndDate' "
                    + "                 FROM Stage "
                    + "                 WHERE ProjectId = @ProjectId AND Name != N'Giai đoạn thanh toán nợ') AS X ON S.EndDate = X.EndDate "
                    + "         WHERE " 
                    + "             S.ProjectId = @ProjectId ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Stage>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT
        public async Task<int> CountAllStagesByProjectId(Guid projectId, string? status)
        {
            try
            {
                var parameters = new DynamicParameters();

                string whereClause = " WHERE ProjectId = @ProjectId ";

                if (status != null)
                {
                    whereClause = whereClause + " AND Status = @Status ";
                    parameters.Add("Status", status, DbType.String);
                }

                var query = "SELECT COUNT(*) FROM Stage " + whereClause;
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
