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
                    + "         Priority, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @Title, "
                    + "         @Link, "
                    + "         @Content, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @Priority, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy) ";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("Priority", projectEntityDTO.Priority, DbType.Int16);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.CreateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //CREATE BY ID
        public async Task<int> CreateProjectEntityById(ProjectEntity projectEntityDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectEntity ("
                    + "         Id, "
                    + "         ProjectId, "
                    + "         Title, "
                    + "         Link, "
                    + "         Content, "
                    + "         Description, "
                    + "         Type, "
                    + "         Priority, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy) "
                    + "     VALUES ( "
                    + "         @Id, "
                    + "         @ProjectId, "
                    + "         @Title, "
                    + "         @Link, "
                    + "         @Content, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @Priority, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy) ";

                var parameters = new DynamicParameters();
                parameters.Add("Id", projectEntityDTO.Id, DbType.Guid);
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("Priority", projectEntityDTO.Priority, DbType.Int16);
                parameters.Add("CreateDate", projectEntityDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", projectEntityDTO.UpdateDate, DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectEntityById(Guid projectEntityId)
        {
            try
            {
                var query = "DELETE FROM "
                    + "         ProjectEntity "
                    + "     WHERE "
                    + "         Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectEntityId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        //public async Task<List<ProjectEntity>> GetAllProjectEntities(int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        if (pageIndex != 0 && pageSize != 0)
        //        {
        //            var query = "WITH X AS ( "
        //            + "         SELECT "
        //            + "             ROW_NUMBER() OVER ( "
        //            + "                 ORDER BY "
        //            + "                     ProjectId ASC, "
        //            + "                     Priority ASC ) AS Num, "
        //            + "             * "
        //            + "         FROM ProjectEntity "
        //            + "         ) "
        //            + "     SELECT "
        //            + "         Id, "
        //            + "         ProjectId, "
        //            + "         Title, "
        //            + "         Link, "
        //            + "         Content, "
        //            + "         Description, "
        //            + "         Priority, "
        //            + "         Type, "
        //            + "         CreateDate, "
        //            + "         CreateBy, "
        //            + "         UpdateDate, "
        //            + "         UpdateBy "
        //            + "     FROM "
        //            + "         X "
        //            + "     WHERE "
        //            + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
        //            + "         AND @PageIndex * @PageSize";
        //            var parameters = new DynamicParameters();
        //            parameters.Add("PageIndex", pageIndex, DbType.Int16);
        //            parameters.Add("PageSize", pageSize, DbType.Int16);
        //            using var connection = CreateConnection();
        //            return (await connection.QueryAsync<ProjectEntity>(query, parameters)).ToList();
        //        }
        //        else
        //        {
        //            var query = "SELECT * FROM ProjectEntity ORDER BY ProjectId ASC, Priority ASC";
        //            using var connection = CreateConnection();
        //            return (await connection.QueryAsync<ProjectEntity>(query)).ToList();
        //        }               
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}

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
                LoggerService.Logger(e.ToString());
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
                    + "         Title = ISNULL(@Title, Title), "
                    + "         Link = ISNULL(@Link, Link), "
                    + "         Content = ISNULL(@Content, Content), "
                    + "         Description = ISNULL(@Description, Description), "
                    + "         Type = ISNULL(@Type, Type), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectEntityId, DbType.Guid);

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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ProjectEntity>> GetProjectEntityByProjectIdAndType(Guid projectId, string type)
        {
            try
            {
                var parameters = new DynamicParameters();

                string whereCondition = " WHERE ProjectId = @ProjectId ";
                parameters.Add("ProjectId", projectId, DbType.Guid);

                if (type != null)
                {
                    whereCondition = whereCondition + " AND Type = @Type ";
                    parameters.Add("Type", type, DbType.String);
                }
                    
                string query = "SELECT " 
                    + "             * " 
                    + "         FROM " 
                    + "             ProjectEntity "
                    +           whereCondition 
                    + "         ORDER BY "
                    + "             Type ASC, Priority ASC ";                             

                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectEntity>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> CountProjectEntityByProjectIdAndType(Guid projectId, string type)
        {
            try
            {
                string query = "SELECT "
                    + "             COUNT(*) "
                    + "         FROM "
                    + "             ProjectEntity "
                    + "         WHERE "
                    + "             ProjectId = @ProjectId " 
                    + "             AND Type = @Type ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("Type", type, DbType.String);
                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //DELETE BY TYPE AND PROJECT_ID
        public async Task<int> DeleteProjectEntityByProjectIdAndType(Guid projectId, string type)
        {
            try
            {
                var query = "DELETE FROM "
                    + "         ProjectEntity "
                    + "     WHERE "
                    + "         ProjectId = @ProjectId "
                    + "         AND Type = @Type";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("Type", type, DbType.String);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateProjectEntityPriority(Guid projectEntityId, int priority, Guid? updaterId)
        {
            try
            {
                var query = "UPDATE ProjectEntity "
                    + "     SET "
                    + "         Priority = @Priority, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Priority", priority, DbType.Int16);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", updaterId, DbType.Guid);
                parameters.Add("Id", projectEntityId, DbType.Guid);

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
                    + "         UpdateBy ) "
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
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Id", projectEntityDTO.Id, DbType.Guid);
                parameters.Add("ProjectId", projectEntityDTO.ProjectId, DbType.Guid);
                parameters.Add("Title", projectEntityDTO.Title, DbType.String);
                parameters.Add("Link", projectEntityDTO.Link, DbType.String);
                parameters.Add("Content", projectEntityDTO.Content, DbType.String);
                parameters.Add("Description", projectEntityDTO.Description, DbType.String);
                parameters.Add("Type", projectEntityDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", projectEntityDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectEntityDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                connection.Open();
                return connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> DeleteProjectEntityByProjectId(Guid projectId)
        {
            try
            {
                var query = "DELETE FROM "
                    + "         ProjectEntity "
                    + "     WHERE "
                    + "         ProjectId = @ProjectId ";
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

        //UPDATE PROJECT MANAGER CONTACT EXTENSION
        public async Task<int> UpdateProjectManagerContactExtension(Guid managerId, string phoneNum)
        {
            try
            {
                var query = "UPDATE PE "
                    + "     SET "
                    + "         PE.Description = @Description, "
                    + "         PE.UpdateDate = @UpdateDate, "
                    + "         PE.UpdateBy = ISNULL(@UpdateBy, PE.UpdateBy) "
                    + "     FROM " 
                    + "         ProjectEntity PE " 
                    + "         JOIN Project P ON PE.ProjectId = P.Id "
                    + "     WHERE "
                    + "         P.ManagerId = @ManagerId "
                    + "         AND PE.Title = @Title";

                var parameters = new DynamicParameters();
                parameters.Add("Description", "Liên hệ: " + phoneNum, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", managerId, DbType.Guid);
                parameters.Add("ManagerId", managerId , DbType.Guid);
                parameters.Add("Title", "Chủ dự án", DbType.String);

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

        //UPDATE BUSINESS EMAIL EXTENSION
        public async Task<int> UpdateBusinessEmailExtension(Guid businessId, string email)
        {
            try
            {
                var query = "UPDATE PE "
                    + "     SET "
                    + "         PE.Description = @Description, "
                    + "         PE.UpdateDate = @UpdateDate "
                    + "     FROM "
                    + "         ProjectEntity PE "
                    + "         JOIN Project P ON PE.ProjectId = P.Id "
                    + "     WHERE "
                    + "         P.BusinessId = @BusinessId "
                    + "         AND PE.Title = @Title";

                var parameters = new DynamicParameters();
                parameters.Add("Description", "Email: " + email, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("Title", "Doanh nghiệp", DbType.String);

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
