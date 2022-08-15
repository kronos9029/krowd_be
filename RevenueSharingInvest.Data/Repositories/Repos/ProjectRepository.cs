using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                    + "         FieldId, "
                    + "         AreaId, "
                    + "         Name, "
                    //+ "         Image, "
                    + "         Description, "
                    + "         Address, "
                    + "         InvestmentTargetCapital, "
                    //+ "         InvestedCapital, "
                    + "         SharedRevenue, "
                    + "         Multiplier, "
                    + "         Duration, "
                    + "         NumOfStage, "
                    + "         RemainAmount, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         BusinessLicense, "
                    //+ "         ApprovedDate, "
                    //+ "         ApprovedBy, " //Id Admin
                    + "         Status, "
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
                    + "         @FieldId, "
                    + "         @AreaId, "
                    + "         @Name, "
                    //+ "         @Image, "
                    + "         @Description, "
                    + "         @Address, "
                    + "         @InvestmentTargetCapital, "
                    //+ "         @InvestedCapital, "
                    + "         @SharedRevenue, "
                    + "         @Multiplier, "
                    + "         @Duration, "
                    + "         @NumOfStage, "
                    + "         @RemainAmount, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @BusinessLicense, "
                    //+ "         null, "
                    //+ "         null, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", projectDTO.ManagerId, DbType.Guid);
                parameters.Add("BusinessId", projectDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", projectDTO.FieldId, DbType.Guid);
                parameters.Add("AreaId", projectDTO.AreaId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                //parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("Address", projectDTO.Description, DbType.String);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                //parameters.Add("InvestedCapital", projectDTO.InvestedCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("RemainAmount", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("StartDate", Convert.ToDateTime(projectDTO.StartDate), DbType.DateTime);
                parameters.Add("EndDate", Convert.ToDateTime(projectDTO.EndDate), DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("Status", projectDTO.Status, DbType.String);
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
        public async Task<List<Project>> GetAllProjects
            (
            int pageIndex,
            int pageSize,
            string businessId,
            string managerId,
            string areaId,
            string fieldId,
            string investorId,
            string name,
            string status,
            string temp_field_role
            )
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";
                var isDeletedCondition = " AND IsDeleted = 0 ";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var managerIdCondition = " AND ManagerId = @ManagerId ";
                var areaIdCondition = " AND AreaId = @AreaId ";
                var fieldIdCondition = " AND FieldId = @FieldId ";
                var investorIdCondition = " Id IN (SELECT DISTINCT ProjectId FROM Investment WHERE InvestorId = @InvestorId) ";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";
                var statusCondition = " AND Status = @Status ";

                if (temp_field_role.Equals("ADMIN"))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("BUSINESS"))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("PROJECT"))
                {
                    whereCondition = whereCondition + managerIdCondition;
                    parameters.Add("ManagerId", Guid.Parse(managerId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("INVESTOR"))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }

                    if (investorId != null)
                    {
                        whereCondition = whereCondition + investorIdCondition;
                        parameters.Add("InvestorId", Guid.Parse(investorId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "') "
                        + isDeletedCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("GUEST"))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                            + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "') "
                            + isDeletedCondition;
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

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
                    + whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         ManagerId, " //Id của chủ dự án
                    + "         BusinessId, " //Id của Business
                    + "         FieldId, "
                    + "         AreaId, "
                    + "         Name, "
                    + "         Image, "
                    + "         Description, "
                    + "         Address, "
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
                    + "         ApprovedDate, "
                    + "         ApprovedBy, " //Id Admin
                    + "         Status, "
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
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Project>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Project " + whereCondition + " ORDER BY BusinessId ASC, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Project>(query, parameters)).ToList();
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
                    + "         FieldId = @FieldId, "
                    + "         AreaId = @AreaId, "
                    + "         Name = @Name, "
                    + "         Image = @Image, "
                    + "         Description = @Description, "
                    + "         Address = @Address, "
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
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", projectDTO.ManagerId, DbType.Guid);
                parameters.Add("BusinessId", projectDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", projectDTO.FieldId, DbType.Guid);
                parameters.Add("AreaId", projectDTO.AreaId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("Address", projectDTO.Description, DbType.String);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("InvestedCapital", projectDTO.InvestedCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("RemainAmount", projectDTO.RemainAmount, DbType.Double);
                parameters.Add("StartDate", Convert.ToDateTime(projectDTO.StartDate), DbType.DateTime);
                parameters.Add("EndDate", Convert.ToDateTime(projectDTO.EndDate), DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", projectDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", projectId, DbType.Guid);

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

        public async Task<int> CountProject
        (
            string businessId,
            string managerId,
            string areaId,
            string fieldId,
            string investorId,
            string name,
            string status,
            string temp_field_role
        )
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";
                var isDeletedCondition = " AND IsDeleted = 0 ";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var managerIdCondition = " AND ManagerId = @ManagerId ";
                var areaIdCondition = " AND AreaId = @AreaId ";
                var fieldIdCondition = " AND FieldId = @FieldId ";
                var investorIdCondition = " Id IN (SELECT DISTINCT ProjectId FROM Investment WHERE InvestorId = @InvestorId) ";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";
                var statusCondition = " AND Status = @Status ";

                if (temp_field_role.Equals(RoleEnum.ADMIN.ToString()))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals(RoleEnum.BUSINESS_MANAGER.ToString()))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals(RoleEnum.PROJECT_OWNER.ToString()))
                {
                    whereCondition = whereCondition + managerIdCondition;
                    parameters.Add("ManagerId", Guid.Parse(managerId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(2) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals(RoleEnum.INVESTOR.ToString()))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }

                    if (investorId != null)
                    {
                        whereCondition = whereCondition + investorIdCondition;
                        parameters.Add("InvestorId", Guid.Parse(investorId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(5) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "') "
                        + isDeletedCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("GUEST"))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }
                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                            + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(3) + "') "
                            + isDeletedCondition;
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (temp_field_role.Equals("GUEST"))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (fieldId != null)
                    {
                        whereCondition = whereCondition + fieldIdCondition;
                        parameters.Add("FieldId", Guid.Parse(fieldId), DbType.Guid);
                    }

                    whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(1) + "') "
                        + isDeletedCondition;

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                var query = "SELECT COUNT(*) FROM Project " + whereCondition;

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<List<BusinessProjectDTO>> GetBusinessProjectsToAuthor(Guid businessId)
        {
            try
            {
                var query = "SELECT p.BusinessId, p.Id AS ProjectId, p.Name AS ProjectName, p.ManagerId " +
                    "FROM Project p RIGHT JOIN [Business] b ON p.BusinessId = b.Id " +
                    "WHERE b.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<BusinessProjectDTO>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

    public enum RoleEnum
    {
        ADMIN,
        INVESTOR,
        BUSINESS_MANAGER,
        PROJECT_OWNER
    }
}
