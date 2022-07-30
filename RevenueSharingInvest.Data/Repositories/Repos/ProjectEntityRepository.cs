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
        public async Task<string> CreateProjectEntity(ProjectEntity projectEntityDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectEntity ("
                    + "         ProjectId, "
                    + "         Title, "
                    + "         Link, "
                    + "         Content, "
                    + "         Description, "
                    + "         Type, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @Title, "
                    + "         @Link, "
                    + "         @Content, "
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
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
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
        public async Task<List<ProjectEntity>> GetAllProjectEntities(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     ProjectId ASC ) AS Num, "
                    + "             * "
                    + "         FROM ProjectEntity "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         ProjectId, "
                    + "         Title, "
                    + "         Link, "
                    + "         Content, "
                    + "         Description, "
                    + "         Type, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<ProjectEntity>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM ProjectEntity WHERE IsDeleted = 0 ORDER BY ProjectId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<ProjectEntity>(query)).ToList();
                }               
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
                    + "         Link = @Link, "
                    + "         Content = @Content, "
                    + "         Description = @Description, "
                    + "         Type = @Type, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);
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

        //CLEAR DATA
        public async Task<int> ClearAllProjectEntityData()
        {
            try
            {
                var query = "DELETE FROM ProjectEntity";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ProjectEntity>> GetProjectEntityByTypeAndProjectId(Guid projectId, string type)
        {
            try
            {
                string query = "SELECT " 
                    + "             * " 
                    + "         FROM " 
                    + "             ProjectEntity "
                    + "         WHERE " 
                    + "             ProjectId = @ProjectId AND Type = @Type AND IsDeleted = 0";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("Type", type, DbType.String);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectEntity>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public Task<int> CreateProjectEntityFromFirebase(ProjectEntity projectEntityDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectEntity ("
                    + "         Id,"
                    + "         ProjectId, "
                    + "         Title, "
                    + "         Link, "
                    + "         Content, "
                    + "         Description, "
                    + "         Type, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @Id, "
                    + "         @ProjectId, "
                    + "         @Title, "
                    + "         @Link, "
                    + "         @Content, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Id", projectEntityDTO.Id, DbType.Guid);
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
