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
    public class ProjectEntityRepository : BaseRepository, IProjectEntityRepository
    {
        public ProjectEntityRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateProjectEntity(ProjectEntity projectEntityDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectEntity ("
                    + "         ProjectId, "
                    + "         Title, "
                    + "         Image, "
                    + "         Description, "
                    + "         Type, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @Title, "
                    + "         @Image, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Image", projectEntityDTO.Image, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectEntityById(Guid projectEntityId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE ProjectEntity "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectEntityId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<ProjectEntity>> GetAllProjectEntitys()
        {
            try
            {
                string query = "SELECT * FROM ProjectEntity";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectEntity>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<ProjectEntity> GetProjectEntityById(Guid projectEntityId)
        {
            try
            {
                string query = "SELECT * FROM ProjectEntity WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectEntityId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ProjectEntity>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateProjectEntity(ProjectEntity projectEntityDTO, Guid projectEntityId)
        {
            try
            {
                var query = "UPDATE ProjectEntity "
                    + "     SET "
                    + "         ProjectId = @ProjectId, "
                    + "         Title = @Title, "
                    + "         Image = @Image, "
                    + "         Description = @Description, "
                    + "         Type = @Type, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Image", projectEntityDTO.Image, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("CreateDate", projectEntityDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", projectEntityDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", projectEntityId, DbType.Guid);

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
