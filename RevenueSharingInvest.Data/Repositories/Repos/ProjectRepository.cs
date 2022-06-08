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
    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        public ProjectRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateProject(Project projectDTO)
        {
            try
            {
                var query = "INSERT INTO Project ("
                    + "         ManagerId, " //Id của chủ dự án
                    + "         BusinessId, " //Id của Business
                    + "         Name, "
                    + "         Image, "
                    + "         Description, "
                    + "         FieldId, "
                    + "         Address, "
                    + "         ProjectId, "
                    + "         InvestmentTargetCapital, "
                    + "         InvestedCapital, "
                    + "         SharedRevenue, "
                    + "         Multiplier, "
                    + "         Duration, "
                    + "         NumOfStage, "
                    + "         RemainAmount, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         BusinessLicense, "
                    + "         Status, "
                    + "         ApprovedDate, "
                    + "         ApprovedBy, " //Id Admin
                    + "         CreateDate, "
                    + "         CreateBy, " //Id Business Manager
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ManagerId, "
                    + "         @BusinessId, "
                    + "         @Name, "
                    + "         @Image, "
                    + "         @Description, "
                    + "         @FieldId, "
                    + "         @Address, "
                    + "         @ProjectId, "
                    + "         @InvestmentTargetCapital, "
                    + "         @InvestedCapital, "
                    + "         @SharedRevenue, "
                    + "         @Multiplier, "
                    + "         @Duration, "
                    + "         @NumOfStage, "
                    + "         @RemainAmount, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @BusinessLicense, "
                    + "         @Status, "
                    + "         @ApprovedDate, "
                    + "         @ApprovedBy, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", projectDTO.ManagerId, DbType.Guid);
                parameters.Add("BusinessId", projectDTO.BusinessId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("FieldId", projectDTO.FieldId, DbType.Guid);
                parameters.Add("Address", projectDTO.Description, DbType.String);
                parameters.Add("AreaId", projectDTO.AreaId, DbType.Guid);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("InvestedCapital", projectDTO.InvestedCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("RemainAmount", projectDTO.RemainAmount, DbType.Double);
                parameters.Add("StartDate", projectDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", projectDTO.EndDate, DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("Status", projectDTO.Status, DbType.Int16);
                parameters.Add("ApprovedDate", projectDTO.ApprovedDate, DbType.DateTime);
                parameters.Add("ApprovedBy", projectDTO.ApprovedBy, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectById(Guid projectId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE Project "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Project>> GetAllProjects(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     BusinessId ASC, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Project "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         ManagerId, " //Id của chủ dự án
                    + "         BusinessId, " //Id của Business
                    + "         Name, "
                    + "         Image, "
                    + "         Description, "
                    + "         FieldId, "
                    + "         Address, "
                    + "         ProjectId, "
                    + "         InvestmentTargetCapital, "
                    + "         InvestedCapital, "
                    + "         SharedRevenue, "
                    + "         Multiplier, "
                    + "         Duration, "
                    + "         NumOfStage, "
                    + "         RemainAmount, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         BusinessLicense, "
                    + "         Status, "
                    + "         ApprovedDate, "
                    + "         ApprovedBy, " //Id Admin
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
                    return (await connection.QueryAsync<Project>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Project WHERE IsDeleted = 0 ORDER BY BusinessId ASC, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Project>(query)).ToList();
                }              
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Project> GetProjectById(Guid projectId)
        {
            try
            {
                string query = "SELECT * FROM Project WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Project>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateProject(Project projectDTO, Guid projectId)
        {
            try
            {
                var query = "UPDATE Project "
                    + "     SET "
                    + "         ManagerId = @ManagerId, "
                    + "         BusinessId = @BusinessId, "
                    + "         Name = @Name, "
                    + "         Image = @Image, "
                    + "         Description = @Description, "
                    + "         FieldId = @FieldId, "
                    + "         Address = @Address, "
                    + "         ProjectId = @ProjectId, "
                    + "         InvestmentTargetCapital = @InvestmentTargetCapital, "
                    + "         InvestedCapital = @InvestedCapital, "
                    + "         SharedRevenue = @SharedRevenue, "
                    + "         Multiplier = @Multiplier, "
                    + "         Duration = @Duration, "
                    + "         NumOfStage = @NumOfStage, "
                    + "         RemainAmount = @RemainAmount, "
                    + "         StartDate = @StartDate, "
                    + "         EndDate = @EndDate, "
                    + "         BusinessLicense = @BusinessLicense, "
                    + "         Status = @Status, "
                    + "         ApprovedDate = @ApprovedDate, "
                    + "         ApprovedBy = @ApprovedBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted,"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", projectDTO.ManagerId, DbType.Guid);
                parameters.Add("BusinessId", projectDTO.BusinessId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("FieldId", projectDTO.FieldId, DbType.Guid);
                parameters.Add("Address", projectDTO.Description, DbType.String);
                parameters.Add("AreaId", projectDTO.AreaId, DbType.Guid);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("InvestedCapital", projectDTO.InvestedCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("RemainAmount", projectDTO.RemainAmount, DbType.Double);
                parameters.Add("StartDate", projectDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", projectDTO.EndDate, DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("Status", projectDTO.Status, DbType.Int16);
                parameters.Add("ApprovedDate", projectDTO.ApprovedDate, DbType.DateTime);
                parameters.Add("ApprovedBy", projectDTO.ApprovedBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", projectDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", projectDTO.Id, DbType.Guid);

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

        //CLEAR DATA***
        public async Task<int> ClearAllProjectData()
        {
            try
            {
                var query = "DELETE FROM Project";
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
