using Dapper;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Project = RevenueSharingInvest.Data.Models.Entities.Project;

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
                    + "         Description, "
                    + "         Address, "
                    + "         InvestmentTargetCapital, "
                    + "         SharedRevenue, "
                    + "         Multiplier, "
                    + "         Duration, "
                    + "         NumOfStage, "
                    + "         RemainAmount, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         BusinessLicense, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, " //Id Business Manager
                    + "         UpdateDate, "
                    + "         UpdateBy," 
                    + "         AccessKey ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ManagerId, "
                    + "         @BusinessId, "
                    + "         @FieldId, "
                    + "         @AreaId, "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @Address, "
                    + "         @InvestmentTargetCapital, "
                    + "         @SharedRevenue, "
                    + "         @Multiplier, "
                    + "         @Duration, "
                    + "         @NumOfStage, "
                    + "         @RemainAmount, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @BusinessLicense, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy," 
                    + "         @AccessKey )";

                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", projectDTO.ManagerId, DbType.Guid);
                parameters.Add("BusinessId", projectDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", projectDTO.FieldId, DbType.Guid);
                parameters.Add("AreaId", projectDTO.AreaId, DbType.Guid);
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("Address", projectDTO.Address, DbType.String);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("RemainAmount", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("StartDate", Convert.ToDateTime(projectDTO.StartDate), DbType.DateTime);
                parameters.Add("EndDate", Convert.ToDateTime(projectDTO.EndDate), DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("Status", projectDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", projectDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.CreateBy, DbType.Guid);
                parameters.Add("AccessKey", projectDTO.AccessKey, DbType.String);


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
        public async Task<int> DeleteProjectById(Guid projectId)//thiếu para UpdateBy
        {
            try
            {
                var query = "DELETE FROM Project "
                    + "     WHERE "
                    + "         Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
            List<string> listFieldId,
            string name,
            string status,
            string roleId
            )
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var managerIdCondition = " AND ManagerId = @ManagerId ";
                var areaIdCondition = " AND AreaId = @AreaId ";
                var fieldIdCondition = "";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";
                var statusCondition = " AND Status = @Status ";

                if (roleId.Equals("") || roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
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
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                            + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "') ";
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
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
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    whereCondition = whereCondition + managerIdCondition;
                    parameters.Add("ManagerId", Guid.Parse(managerId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
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
                    + "         UpdateBy "
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
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
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
                    + "         Name = ISNULL(@Name, Name), "
                    + "         Image = ISNULL(@Image, Image), "
                    + "         Description = ISNULL(@Description, Description), "
                    + "         Address = ISNULL(@Address, Address), "
                    + "         InvestmentTargetCapital = ISNULL(@InvestmentTargetCapital, InvestmentTargetCapital), "
                    + "         RemainAmount = ISNULL(@RemainAmount, RemainAmount), "
                    + "         SharedRevenue = ISNULL(@SharedRevenue, SharedRevenue), "
                    + "         Multiplier = ISNULL(@Multiplier, Multiplier), "
                    + "         Duration = ISNULL(@Duration, Duration), "
                    + "         NumOfStage = ISNULL(@NumOfStage, NumOfStage), "
                    + "         StartDate = ISNULL(@StartDate, StartDate), "
                    + "         EndDate = ISNULL(@EndDate, EndDate), "
                    + "         BusinessLicense = ISNULL(@BusinessLicense, BusinessLicense), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", projectDTO.Name, DbType.String);
                parameters.Add("Image", projectDTO.Image, DbType.String);
                parameters.Add("Description", projectDTO.Description, DbType.String);
                parameters.Add("Address", projectDTO.Address, DbType.String);
                parameters.Add("InvestmentTargetCapital", projectDTO.InvestmentTargetCapital, DbType.Double);
                parameters.Add("RemainAmount", projectDTO.RemainAmount, DbType.Double);
                parameters.Add("InvestedCapital", projectDTO.InvestedCapital, DbType.Double);
                parameters.Add("SharedRevenue", projectDTO.SharedRevenue, DbType.Double);
                parameters.Add("Multiplier", projectDTO.Multiplier, DbType.Double);
                parameters.Add("Duration", projectDTO.Duration, DbType.Int16);
                parameters.Add("NumOfStage", projectDTO.NumOfStage, DbType.Int16);
                parameters.Add("StartDate", Convert.ToDateTime(projectDTO.StartDate), DbType.DateTime);
                parameters.Add("EndDate", Convert.ToDateTime(projectDTO.EndDate), DbType.DateTime);
                parameters.Add("BusinessLicense", projectDTO.BusinessLicense, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectId, DbType.Guid);

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
        
        //UPDATE IMAGE
        public async Task<int> UpdateProjectImage(string url, Guid projectId)
        {
            try
            {
                var query = "UPDATE Project SET Image = @Image WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Image", url, DbType.String);
                parameters.Add("Id", projectId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT
        public async Task<int> CountProject
        (
            string businessId,
            string managerId,
            string areaId,
            List<string> listFieldId,
            string name,
            string status,
            string roleId
        )
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var managerIdCondition = " AND ManagerId = @ManagerId ";
                var areaIdCondition = " AND AreaId = @AreaId ";
                var fieldIdCondition = "";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";
                var statusCondition = " AND Status = @Status ";

                if (roleId.Equals("") || roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
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
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                            + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "') ";
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
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
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    whereCondition = whereCondition + managerIdCondition;
                    parameters.Add("ManagerId", Guid.Parse(managerId), DbType.Guid);

                    if (areaId != null)
                    {
                        whereCondition = whereCondition + areaIdCondition;
                        parameters.Add("AreaId", Guid.Parse(areaId), DbType.Guid);
                    }
                    if (listFieldId != null && listFieldId.Count != 0)
                    {
                        foreach (string fieldId in listFieldId)
                        {
                            fieldIdCondition = fieldIdCondition + " OR FieldId = '" + fieldId + "' ";
                        }
                        fieldIdCondition = "AND ( " + fieldIdCondition.Substring(3, fieldIdCondition.Length - 3) + " ) ";
                        whereCondition = whereCondition + fieldIdCondition;
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
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                        + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "')";
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                var query = "SELECT COUNT(*) FROM Project " + whereCondition;

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE STATUS
        public async Task<int> UpdateProjectStatus(Guid projectId, string status, Guid updaterId)
        {
            try
            {
                var query = "UPDATE Project "
                    + "     SET "
                    + "         Status = ISNULL(@Status, Status), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Status", status, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", updaterId, DbType.Guid);
                parameters.Add("Id", projectId, DbType.Guid);

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

        //GET INVESTED PROJECTS
        public async Task<List<Project>> GetInvestedProjects(int pageIndex, int pageSize, Guid investorId)
        {
            try 
            {
                var parameters = new DynamicParameters();

                var whereCondition = " WHERE (Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "') "
                + " AND Id IN (SELECT DISTINCT ProjectId FROM Investment WHERE InvestorId = @InvestorId) ";
                parameters.Add("InvestorId", investorId, DbType.Guid);

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
                    + "         UpdateBy "
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }


        //COUNT INVESTED PROJECTS
        public async Task<int> CountInvestedProjects(Guid investorId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = " WHERE (Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(4) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(6) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(7) + "' OR Status = '"
                + Enum.GetNames(typeof(ProjectStatusEnum)).ElementAt(8) + "') "
                + " AND Id IN (SELECT DISTINCT ProjectId FROM Investment WHERE InvestorId = @InvestorId) ";
                parameters.Add("InvestorId", investorId, DbType.Guid);

                var query = "SELECT COUNT(*) FROM Project " + whereCondition;

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE INVESTED CAPITAL AND REMAIN AMOUNT
        public async Task<int> UpdateProjectInvestedCapitalAndRemainAmount(Guid projectId, double investedAmount, Guid updateBy)
        {
            try
            {
                var query = "UPDATE Project "
                    + "     SET "
                    + "         RemainAmount = ISNULL(RemainAmount - @InvestedAmount, RemainAmount), "
                    + "         InvestedCapital = ISNULL(InvestedCapital + @InvestedAmount, InvestedCapital), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestedAmount", investedAmount, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", updateBy, DbType.Guid);
                parameters.Add("Id", projectId, DbType.Guid);

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

        //APPROVE PROJECT
        public async Task<int> ApproveProject(Guid projectId, Guid updateBy)
        {
            try
            {
                var query = "UPDATE Project "
                    + "     SET "
                    + "         Status = ISNULL(@Status, Status), "
                    + "         ApprovedDate = ISNULL(@ApprovedDate, ApprovedDate), "
                    + "         ApprovedBy = ISNULL(@ApprovedBy, ApprovedBy), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Status", ProjectStatusEnum.WAITING_TO_PUBLISH.ToString(), DbType.String);
                parameters.Add("ApprovedDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("ApprovedBy", updateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", updateBy, DbType.Guid);
                parameters.Add("Id", projectId, DbType.Guid);

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

        public async Task<IntegrateInfo> GetIntegrateInfoByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT u.Id AS UserId, p.Id AS ProjectId, u.SecretKey, p.AccessKey FROM Project p JOIN [User] u ON p.ManagerId = u.Id WHERE p.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<IntegrateInfo>(query, parameters);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
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
