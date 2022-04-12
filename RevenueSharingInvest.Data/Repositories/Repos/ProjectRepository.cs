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
        public async Task<int> CreateProject(Project projectDTO)
        {
            try
            {
                var query = "INSERT INTO Project ("
                    + "         ManagerId, " //Id của chủ dự án
                    + "         BusinessId, " //Id của Business
                    + "         Name, "
                    + "         Image, "
                    + "         Description, "
                    + "         Category, "
                    + "         Address, "
                    + "         AreaId, "
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
                    + "     VALUES ( "
                    + "         @ManagerId, "
                    + "         @BusinessId, "
                    + "         @Name, "
                    + "         @Image, "
                    + "         @Description, "
                    + "         @Category, "
                    + "         @Address, "
                    + "         @AreaId, "
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
                parameters.Add("BusinessId", projectDTO .BusinessId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("Category", projectDTO.Category, DbType.Int16);
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
                parameters.Add("CreateDate", projectDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", projectDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", projectDTO.UpdateDate, DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<Project>> GetAllProjects()
        {
            try{
                string query = "SELECT * FROM Project";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Project>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
