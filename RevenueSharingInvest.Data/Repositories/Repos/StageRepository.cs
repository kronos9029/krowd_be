using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
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
        public async Task<int> CreateStage(Stage stageDTO)
        {
            try
            {
                var query = "INSERT INTO Stage ("
                    + "         Name, "
                    + "         ProjectId, "
                    + "         Description, "
                    + "         Percents, "
                    + "         OpenMonth, "
                    + "         CloseMonth, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @ProjectId, "
                    + "         @Description, "
                    + "         @Percents, "
                    + "         @OpenMonth, "
                    + "         @CloseMonth, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("ProjectId", stageDTO.ProjectId, DbType.Guid);
                parameters.Add("Description", stageDTO.Description, DbType.String);
                parameters.Add("Percents", stageDTO.Percents, DbType.Double);
                parameters.Add("OpenMonth", stageDTO.OpenMonth, DbType.Int16);
                parameters.Add("CloseMonth", stageDTO.CloseMonth, DbType.Int16);
                parameters.Add("Status", stageDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", stageDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteStageById(Guid stageId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE Stage "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", stageId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Stage>> GetAllStages()
        {
            try
            {
                string query = "SELECT * FROM Stage";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Stage>(query)).ToList();
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
                    + "         Name = @Name, "
                    + "         ProjectId = @ProjectId, "
                    + "         Description = @Description, "
                    + "         Percents = @Percents, "
                    + "         OpenMonth = @OpenMonth, "
                    + "         CloseMonth = @CloseMonth, "
                    + "         Status = @Status, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", stageDTO.Name, DbType.String);
                parameters.Add("ProjectId", stageDTO.ProjectId, DbType.Guid);
                parameters.Add("Description", stageDTO.Description, DbType.String);
                parameters.Add("Percents", stageDTO.Percents, DbType.Double);
                parameters.Add("OpenMonth", stageDTO.OpenMonth, DbType.Int16);
                parameters.Add("CloseMonth", stageDTO.CloseMonth, DbType.Int16);
                parameters.Add("Status", stageDTO.Status, DbType.String);
                parameters.Add("CreateDate", stageDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", stageDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", stageDTO.IsDeleted, DbType.Boolean);
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
    }
}
