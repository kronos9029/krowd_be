using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
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
                    //+ "         Percents, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @ProjectId, "
                    //+ "         @Percents, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("ProjectId", stageDTO.ProjectId, DbType.Guid);
                //parameters.Add("Percents", stageDTO.Percents, DbType.Double);
                parameters.Add("StartDate", stageDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", stageDTO.EndDate, DbType.DateTime);
                parameters.Add("Status", stageDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", stageDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteStageById(Guid stageId)
        {
            try
            {
                var query = "DELETE FROM Stage WHERE Id = @Id ";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", stageId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Stage>> GetAllStagesByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT * FROM Stage WHERE ProjectId = @ProjectId ORDER BY CreateDate ASC";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Stage>(query, parameters)).ToList();                        
            }
            catch (Exception e)
            {
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
                    + "         StartDate = ISNULL(@StartDate, StartDate), "
                    + "         EndDate = ISNULL(@EndDate, EndDate), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("Description", stageDTO.Description, DbType.String);
                parameters.Add("StartDate", stageDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", stageDTO.EndDate, DbType.DateTime);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", stageId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //CLEAR DATA
        public async Task<int> ClearAllStageData()
        {
            try
            {
                var query = "DELETE FROM Stage";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
