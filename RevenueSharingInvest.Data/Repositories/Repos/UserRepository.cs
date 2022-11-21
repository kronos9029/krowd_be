using Dapper;
using Microsoft.Extensions.Configuration;
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
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateUser(User userDTO)
        {
            try
            {
                var query = "INSERT INTO [User] ("
                    + "         RoleId, "
                    + "         BusinessId, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         Image, "
                    + "         Email, "                    
                    + "         Status, "                    
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy," 
                    + "         SecretKey) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @RoleId, "
                    + "         @BusinessId, "
                    + "         @LastName, "
                    + "         @FirstName, "
                    + "         @Image, "
                    + "         @Email, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy," 
                    + "         @SecretKey)";

                var parameters = new DynamicParameters();
                parameters.Add("RoleId", userDTO.RoleId, DbType.Guid);
                parameters.Add("BusinessId", userDTO.BusinessId, DbType.Guid);
                parameters.Add("LastName", userDTO.LastName, DbType.String);
                parameters.Add("FirstName", userDTO.FirstName, DbType.String);
                parameters.Add("Image", userDTO.Image, DbType.String);
                parameters.Add("Email", userDTO.Email, DbType.String);
                parameters.Add("Status", userDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", userDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", userDTO.CreateBy, DbType.Guid);
                parameters.Add("SecretKey", userDTO.SecretKey??="", DbType.String);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ADMIN
        public async Task<List<User>> GetAllAdmins()
        {
            try
            {
                var query = "SELECT * FROM [User] WHERE RoleId = @Admin";
                var parameters = new DynamicParameters();
                parameters.Add("Admin", Guid.Parse(RoleDictionary.role.GetValueOrDefault("ADMIN")), DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<User>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT ADMIN
        public async Task<int> CountAllAdmins()
        {
            try
            {
                var query = "SELECT COUNT(*) FROM [User] WHERE RoleId = @Admin";
                var parameters = new DynamicParameters();
                parameters.Add("Admin", Guid.Parse(RoleDictionary.role.GetValueOrDefault("ADMIN")), DbType.Guid);
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BUSINESS_MANAGER
        public async Task<List<User>> GetAllBusinesManagers(int pageIndex, int pageSize, Guid? businessId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var whereClause = " AND RoleId = @Business_Manager ";
                parameters.Add("Business_Manager", Guid.Parse(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")), DbType.Guid);
                var businessIdCondtion = " AND BusinessId = @BusinessId ";
                var statusCondtion = " AND Status = @Status ";

                if (businessId != null)
                {
                    whereClause = whereClause + businessIdCondtion;
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     CreateDate DESC ) AS Num, "
                    + "             * "
                    + "         FROM [User] "
                    + whereClause + " ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         BusinessId, "
                    + "         RoleId, "
                    + "         Description, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         PhoneNum, "
                    + "         Image, "
                    + "         IdCard, "
                    + "         Email, "
                    + "         Gender, "
                    + "         DateOfBirth, "
                    + "         TaxIdentificationNumber, "
                    + "         City, "
                    + "         District, "
                    + "         Address, "
                    + "         BankName, "
                    + "         BankAccount, "
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
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM [User] " + whereClause + " ORDER BY CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT BUSINESS_MANAGER
        public async Task<int> CountAllBusinesManagers(Guid? businessId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var whereClause = " AND RoleId = @Business_Manager ";
                parameters.Add("Business_Manager", Guid.Parse(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")), DbType.Guid);
                var businessIdCondtion = " AND BusinessId = @BusinessId ";
                var statusCondtion = " AND Status = @Status ";

                if (businessId != null)
                {
                    whereClause = whereClause + businessIdCondtion;
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                var query = "SELECT COUNT(*) FROM [User] " + whereClause;
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET PROJECT_MANAGER
        public async Task<List<User>> GetAllProjectManagers(int pageIndex, int pageSize, Guid? businessId, Guid? projectId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var selectClause = " U.Id, "
                    + "         U.BusinessId, "
                    + "         U.RoleId, "
                    + "         U.Description, "
                    + "         U.LastName, "
                    + "         U.FirstName, "
                    + "         U.PhoneNum, "
                    + "         U.Image, "
                    + "         U.IdCard, "
                    + "         U.Email, "
                    + "         U.Gender, "
                    + "         U.DateOfBirth, "
                    + "         U.TaxIdentificationNumber, "
                    + "         U.City, "
                    + "         U.District, "
                    + "         U.Address, "
                    + "         U.BankName, "
                    + "         U.BankAccount, "
                    + "         U.Status, "
                    + "         U.CreateDate, "
                    + "         U.CreateBy, "
                    + "         U.UpdateDate, "
                    + "         U.UpdateBy ";
                var fromClause = " FROM [User] U ";
                var whereClause = " AND U.RoleId = @Project_Manager ";
                parameters.Add("Project_Manager", Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")), DbType.Guid);
                var businessIdCondtion = " AND U.BusinessId = @BusinessId ";
                var projectIdCondtion = " AND P.Id = @ProjectId ";
                var statusCondtion = " AND U.Status = @Status ";

                if (businessId != null)
                {
                    whereClause = whereClause + businessIdCondtion;
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                }

                if (projectId != null)
                {
                    whereClause = whereClause + projectIdCondtion;
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    fromClause = " FROM [User] U JOIN Project P ON U.Id = P.ManagerId ";
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     U.CreateDate DESC ) AS Num, "
                    +           selectClause
                    +           fromClause
                    +           whereClause
                    + "         GROUP BY "
                    +               selectClause
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         BusinessId, "
                    + "         RoleId, "
                    + "         Description, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         PhoneNum, "
                    + "         Image, "
                    + "         IdCard, "
                    + "         Email, "
                    + "         Gender, "
                    + "         DateOfBirth, "
                    + "         TaxIdentificationNumber, "
                    + "         City, "
                    + "         District, "
                    + "         Address, "
                    + "         BankName, "
                    + "         BankAccount, "
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
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT " + selectClause + fromClause + whereClause + " GROUP BY " + selectClause + " ORDER BY U.CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT PROJECT_MANAGER
        public async Task<int> CountAllProjectManagers(Guid? businessId, Guid? projectId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var selectClause = " U.Id, "
                    + "         U.BusinessId, "
                    + "         U.RoleId, "
                    + "         U.Description, "
                    + "         U.LastName, "
                    + "         U.FirstName, "
                    + "         U.PhoneNum, "
                    + "         U.Image, "
                    + "         U.IdCard, "
                    + "         U.Email, "
                    + "         U.Gender, "
                    + "         U.DateOfBirth, "
                    + "         U.TaxIdentificationNumber, "
                    + "         U.City, "
                    + "         U.District, "
                    + "         U.Address, "
                    + "         U.BankName, "
                    + "         U.BankAccount, "
                    + "         U.Status, "
                    + "         U.CreateDate, "
                    + "         U.CreateBy, "
                    + "         U.UpdateDate, "
                    + "         U.UpdateBy ";
                var fromClause = " FROM [User] U ";
                var whereClause = " AND U.RoleId = @Project_Manager ";
                parameters.Add("Project_Manager", Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")), DbType.Guid);
                var businessIdCondtion = " AND U.BusinessId = @BusinessId ";
                var projectIdCondtion = " AND P.Id = @ProjectId ";
                var statusCondtion = " AND U.Status = @Status ";

                if (businessId != null)
                {
                    whereClause = whereClause + businessIdCondtion;
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                }

                if (projectId != null)
                {
                    whereClause = whereClause + projectIdCondtion;
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    fromClause = " FROM [User] U JOIN Project P ON U.Id = P.ManagerId ";
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                var query = "SELECT COUNT(*) FROM (SELECT " + selectClause + fromClause + whereClause + " GROUP BY " + selectClause + " ) AS X";
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET INVESTOR
        public async Task<List<User>> GetAllInvestors(int pageIndex, int pageSize, Guid? projectId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var selectClause = " U.Id, "
                    + "         U.BusinessId, "
                    + "         U.RoleId, "
                    + "         U.Description, "
                    + "         U.LastName, "
                    + "         U.FirstName, "
                    + "         U.PhoneNum, "
                    + "         U.Image, "
                    + "         U.IdCard, "
                    + "         U.Email, "
                    + "         U.Gender, "
                    + "         U.DateOfBirth, "
                    + "         U.TaxIdentificationNumber, "
                    + "         U.City, "
                    + "         U.District, "
                    + "         U.Address, "
                    + "         U.BankName, "
                    + "         U.BankAccount, "
                    + "         U.Status, "
                    + "         U.CreateDate, "
                    + "         U.CreateBy, "
                    + "         U.UpdateDate, "
                    + "         U.UpdateBy ";
                var fromClause = " FROM [User] U ";
                var whereClause = " AND U.RoleId = @Investor ";
                parameters.Add("Investor", Guid.Parse(RoleDictionary.role.GetValueOrDefault("INVESTOR")), DbType.Guid);
                var projectIdCondtion = " AND INM.ProjectId = @ProjectId ";
                var statusCondtion = " AND U.Status = @Status ";

                if (projectId != null)
                {
                    whereClause = whereClause + projectIdCondtion;
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    fromClause = " FROM [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     U.CreateDate DESC ) AS Num, "
                    +           selectClause
                    +           fromClause
                    +           whereClause 
                    + "         GROUP BY " 
                    +               selectClause
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         BusinessId, "
                    + "         RoleId, "
                    + "         Description, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         PhoneNum, "
                    + "         Image, "
                    + "         IdCard, "
                    + "         Email, "
                    + "         Gender, "
                    + "         DateOfBirth, "
                    + "         TaxIdentificationNumber, "
                    + "         City, "
                    + "         District, "
                    + "         Address, "
                    + "         BankName, "
                    + "         BankAccount, "
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
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT " + selectClause + fromClause + whereClause + " GROUP BY " + selectClause + " ORDER BY CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT INVESTOR
        public async Task<int> CountAllInvestors(Guid? projectId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                var selectClause = " U.Id, "
                    + "         U.BusinessId, "
                    + "         U.RoleId, "
                    + "         U.Description, "
                    + "         U.LastName, "
                    + "         U.FirstName, "
                    + "         U.PhoneNum, "
                    + "         U.Image, "
                    + "         U.IdCard, "
                    + "         U.Email, "
                    + "         U.Gender, "
                    + "         U.DateOfBirth, "
                    + "         U.TaxIdentificationNumber, "
                    + "         U.City, "
                    + "         U.District, "
                    + "         U.Address, "
                    + "         U.BankName, "
                    + "         U.BankAccount, "
                    + "         U.Status, "
                    + "         U.CreateDate, "
                    + "         U.CreateBy, "
                    + "         U.UpdateDate, "
                    + "         U.UpdateBy ";
                var fromClause = " FROM [User] U ";
                var whereClause = " AND U.RoleId = @Investor ";
                parameters.Add("Investor", Guid.Parse(RoleDictionary.role.GetValueOrDefault("INVESTOR")), DbType.Guid);
                var projectIdCondtion = " AND INM.ProjectId = @ProjectId ";
                var statusCondtion = " AND U.Status = @Status ";

                if (projectId != null)
                {
                    whereClause = whereClause + projectIdCondtion;
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    fromClause = " FROM [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                }

                if (status != null)
                {
                    whereClause = whereClause + statusCondtion;
                    parameters.Add("Status", status, DbType.String);
                }

                whereClause = "WHERE " + whereClause.Substring(4, whereClause.Length - 4);

                var query = "SELECT COUNT(*) FROM (SELECT " + selectClause + fromClause + whereClause + " GROUP BY " + selectClause + " ) AS X";
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        //public async Task<List<User>> GetAllUsers(int pageIndex, int pageSize, string businessId, string projectManagerId, string projectId, string roleId, string status, string thisUserRoleId)
        //{
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        var selectCondition = " * ";
        //        var fromCondition = " [User] ";
        //        var whereCondition = "";
        //        var groupByCondition = " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy";

        //        var businessIdCondition = " AND BusinessId = @BusinessId ";
        //        var roleIdCondition = " AND RoleId = @RoleId ";
        //        var statusCondition = " AND Status = @Status ";

        //        if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
        //        {
        //            if (businessId != null)
        //            {
        //                whereCondition = whereCondition + businessIdCondition;
        //                parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
        //            }
        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (businessId != null)
        //                {
        //                    if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                    {
        //                        selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                        fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                        whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
        //                        parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                        if (status != null)
        //                        {
        //                            whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                            parameters.Add("Status", status, DbType.String);
        //                        }
        //                        else
        //                        {
        //                            whereCondition = whereCondition + " AND (U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                            + groupByCondition;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + roleIdCondition;
        //                        parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }
        //            }

        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
        //        {
        //            whereCondition = whereCondition + businessIdCondition;
        //            parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                {
        //                    selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                    fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                    whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                    if (status != null)
        //                    {
        //                        whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                        parameters.Add("Status", status, DbType.String);
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + " AND (U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                        + groupByCondition;
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }                       
        //            }
        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
        //        {
        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                {
        //                    selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                    fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                    whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE ManagerId = @ManagerId) " + roleIdCondition;
        //                    parameters.Add("ManagerId", Guid.Parse(projectManagerId), DbType.Guid);
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                    if (status != null)
        //                    {
        //                        whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                        parameters.Add("Status", status, DbType.String);
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + " AND (U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                        + groupByCondition;
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }
        //            }
        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        if (pageIndex != 0 && pageSize != 0)
        //        {
        //            var query = "WITH X AS ( "
        //            + "         SELECT "
        //            + "             ROW_NUMBER() OVER ( "
        //            + "                 ORDER BY "
        //            + "                     RoleId, "
        //            + "                     FirstName ASC ) AS Num, "
        //            +               selectCondition
        //            + "         FROM " + fromCondition
        //            +           whereCondition + " ) "
        //            + "     SELECT "
        //            + "         Id, "
        //            + "         BusinessId, "
        //            + "         RoleId, "
        //            + "         Description, "
        //            + "         LastName, "
        //            + "         FirstName, "
        //            + "         PhoneNum, "
        //            + "         Image, "
        //            + "         IdCard, "
        //            + "         Email, "
        //            + "         Gender, "
        //            + "         DateOfBirth, "
        //            + "         TaxIdentificationNumber, "
        //            + "         City, "
        //            + "         District, "
        //            + "         Address, "
        //            + "         BankName, "
        //            + "         BankAccount, "
        //            + "         Status, "
        //            + "         CreateDate, "
        //            + "         CreateBy, "
        //            + "         UpdateDate, "
        //            + "         UpdateBy "
        //            + "     FROM "
        //            + "         X "
        //            + "     WHERE "
        //            + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
        //            + "         AND @PageIndex * @PageSize";

        //            parameters.Add("PageIndex", pageIndex, DbType.Int16);
        //            parameters.Add("PageSize", pageSize, DbType.Int16);
        //            using var connection = CreateConnection();
        //            return (await connection.QueryAsync<User>(query, parameters)).ToList();
        //        }
        //        else
        //        {
        //            var query = "SELECT "+ selectCondition + " FROM " + fromCondition + " " + whereCondition + " ORDER BY RoleId, FirstName ASC";
        //            using var connection = CreateConnection();
        //            return (await connection.QueryAsync<User>(query, parameters)).ToList();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Logger(e.ToString());
        //        throw new Exception(e.Message, e);
        //    }
        //}

        //GET BY ID
        public async Task<User> GetUserById(Guid userId)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }        
        
        //GET BY EMAIL
        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<User> BusinessManagerGetUserById(Guid businessId, Guid userid)
        {
            try
            {

                var query = "SELECT * " +
                            "FROM(SELECT U.Id, " +
                            "            U.BusinessId, " +
                            "            U.RoleId, " +
                            "            U.Description, " +
                            "            U.LastName, " +
                            "            U.FirstName, " +
                            "            U.PhoneNum, " +
                            "            U.Image, " +
                            "            U.IdCard, " +
                            "            U.Email, " +
                            "            U.Gender, " +
                            "            U.DateOfBirth, " +
                            "            U.TaxIdentificationNumber, " +
                            "            U.City, U.District, " +
                            "            U.Address, " +
                            "            U.BankName, " +
                            "            U.BankAccount, " +
                            "            U.Status, " +
                            "            U.CreateDate, " +
                            "            U.CreateBy, " +
                            "            U.UpdateDate, " +
                            "            U.UpdateBy " +
                            "FROM[User] U " +
                            "    JOIN Investor INS ON U.Id = INS.UserId " +
                            "    JOIN Investment INM ON INS.Id = INM.InvestorId " +
                            "WHERE INM.ProjectId IN(SELECT Id " +
                            "                        FROM Project " +
                            "                        WHERE BusinessId = @BusinessId) " +
                            "GROUP BY U.Id, " +
                            "         U.BusinessId, " +
                            "         U.RoleId, " +
                            "         U.Description, " +
                            "         U.LastName, " +
                            "         U.FirstName, " +
                            "         U.PhoneNum, " +
                            "         U.Image, " +
                            "         U.IdCard, " +
                            "         U.Email, " +
                            "         U.Gender, " +
                            "         U.DateOfBirth, " +
                            "         U.TaxIdentificationNumber, " +
                            "         U.City, " +
                            "         U.District, " +
                            "         U.Address, " +
                            "         U.BankName, " +
                            "         U.BankAccount, " +
                            "         U.Status, " +
                            "         U.CreateDate, " +
                            "         U.CreateBy, " +
                            "         U.UpdateDate, " +
                            "         U.UpdateBy " +
                            ") AS X " +
                            "WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("Id", userid, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);

            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<User> ProjectManagerGetUserbyId(Guid managerId, Guid id)
        {
            try
            {
                var query = "SELECT * "+
                            "FROM(SELECT U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy "+
                            "FROM[User] U "+
                            "    JOIN Investor INS ON U.Id = INS.UserId "+
                            "    JOIN Investment INM ON INS.Id = INM.InvestorId "+
                            "   WHERE INM.ProjectId IN(SELECT Id "+
                            "                        FROM Project "+
                            "                        WHERE ManagerId = @ManagerId) "+
                            " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy "+
                            " ) AS X "+
                            " WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ManagerId", managerId, DbType.Guid);
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);

            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

        //UPDATE
        public async Task<int> UpdateUser(User userDTO, Guid userId)
        {
            try
            {
                var query = "UPDATE [User] "
                    + "     SET "
                    + "         BusinessId = ISNULL(@BusinessId, BusinessId), "
                    + "         RoleId = ISNULL(@RoleId, RoleId), "
                    + "         Description = ISNULL(@Description, Description), "
                    + "         LastName = ISNULL(@LastName, LastName), "
                    + "         FirstName = ISNULL(@FirstName, FirstName), "
                    + "         PhoneNum = ISNULL(@PhoneNum, PhoneNum), "
                    + "         Image = ISNULL(@Image, Image), "
                    + "         IdCard = ISNULL(@IdCard, IdCard), "
                    + "         Gender = ISNULL(@Gender, Gender), "
                    + "         DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth), "
                    + "         TaxIdentificationNumber = ISNULL(@TaxIdentificationNumber, TaxIdentificationNumber), "
                    + "         City = ISNULL(@City, City), "
                    + "         District = ISNULL(@District, District), "
                    + "         Address = ISNULL(@Address, Address), "
                    + "         BankName = ISNULL(@BankName, BankName), "
                    + "         BankAccount = ISNULL(@BankAccount, BankAccount), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", userDTO.BusinessId, DbType.Guid);
                parameters.Add("RoleId", userDTO.RoleId, DbType.Guid);
                parameters.Add("Description", userDTO.Description, DbType.String);
                parameters.Add("LastName", userDTO.LastName, DbType.String);
                parameters.Add("FirstName", userDTO.FirstName, DbType.String);
                parameters.Add("PhoneNum", userDTO.PhoneNum, DbType.String);
                parameters.Add("Image", userDTO.Image, DbType.String);
                parameters.Add("IdCard", userDTO.IdCard, DbType.String);
                parameters.Add("Gender", userDTO.Gender, DbType.String);
                parameters.Add("DateOfBirth", userDTO.DateOfBirth, DbType.String);
                parameters.Add("TaxIdentificationNumber", userDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("City", userDTO.City, DbType.String);
                parameters.Add("District", userDTO.District, DbType.String);
                parameters.Add("Address", userDTO.Address, DbType.String);
                parameters.Add("BankName", userDTO.BankName, DbType.String);
                parameters.Add("BankAccount", userDTO.BankAccount, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", userDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

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
        public async Task<int> UpdateUserImage(string url, Guid userId)
        {
            try
            {
                var query = "UPDATE [User] SET  Image = @Image WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Image", url, DbType.String);
                parameters.Add("Id", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<User> GetBusinessManagerByBusinessId(Guid businessId)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE BusinessId = @BusinessId AND RoleId = @RoleId";
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("RoleId", Guid.Parse(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")), DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT
        //public async Task<int> CountUser(string businessId, string projectManagerId, string roleId, string status, string thisUserRoleId)
        //{
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        var selectCondition = " * ";
        //        var fromCondition = " [User] ";
        //        var whereCondition = "";
        //        var groupByCondition = " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";

        //        var businessIdCondition = " AND BusinessId = @BusinessId ";
        //        var roleIdCondition = " AND RoleId = @RoleId ";
        //        var statusCondition = " AND Status = @Status ";

        //        if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
        //        {
        //            if (businessId != null)
        //            {
        //                whereCondition = whereCondition + businessIdCondition;
        //                parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
        //            }
        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (businessId != null)
        //                {
        //                    if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                    {
        //                        selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                        fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                        whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
        //                        parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                        if (status != null)
        //                        {
        //                            whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                            parameters.Add("Status", status, DbType.String);
        //                        }
        //                        else
        //                        {
        //                            whereCondition = whereCondition + " AND (U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                            + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                            + groupByCondition;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + roleIdCondition;
        //                        parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }
        //            }

        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
        //        {
        //            whereCondition = whereCondition + businessIdCondition;
        //            parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                {
        //                    selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                    fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                    whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                    if (status != null)
        //                    {
        //                        whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                        parameters.Add("Status", status, DbType.String);
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + " AND (U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                        + groupByCondition;
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }
        //            }
        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
        //        {
        //            if (status != null)
        //            {
        //                whereCondition = whereCondition + statusCondition;
        //                parameters.Add("Status", status, DbType.String);
        //            }
        //            else
        //            {
        //                whereCondition = whereCondition + " AND (Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
        //                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
        //            }

        //            if (roleId != null)
        //            {
        //                if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
        //                {
        //                    selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy ";
        //                    fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
        //                    whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE ManagerId = @ManagerId) " + roleIdCondition;
        //                    parameters.Add("ManagerId", Guid.Parse(projectManagerId), DbType.Guid);
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

        //                    if (status != null)
        //                    {
        //                        whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
        //                        parameters.Add("Status", status, DbType.String);
        //                    }
        //                    else
        //                    {
        //                        whereCondition = whereCondition + " AND (U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
        //                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
        //                        + groupByCondition;
        //                    }
        //                }
        //                else
        //                {
        //                    whereCondition = whereCondition + roleIdCondition;
        //                    parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
        //                }
        //            }
        //            whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
        //        }

        //        var query = "SELECT COUNT(*) FROM (SELECT " + selectCondition + " FROM " + fromCondition + " " + whereCondition + ") AS X";

        //        using var connection = CreateConnection();
        //        return ((int)connection.ExecuteScalar(query, parameters));
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Logger(e.ToString());
        //        throw new Exception(e.Message);
        //    }
        //}

        public async Task<User> GetProjectManagerByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT "
                    + "         U.* "
                    + "     FROM "
                    + "         [User] U "
                    + "         JOIN Project P ON U.Id = P.ManagerId "
                    + "     WHERE "
                    + "         P.Id = @ProjectId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<User>> GetProjectMembers(Guid projectId)
        {
            try
            {
                var query = "SELECT "
                    + "         U.LastName, U.FirstName, U.Image, MIN(INM.CreateDate)"
                    + "     FROM "
                    + "         [User] U "
                    + "         JOIN Investor INS ON U.Id = INS.UserId "
                    + "         JOIN Investment INM ON INS.Id = INM.InvestorId "
                    + "     WHERE "
                    + "         INM.ProjectId = @ProjectId "
                    + "         AND INM.Status = 'SUCCESS' "
                    + "     GROUP BY U.LastName, U.FirstName, U.Image";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<User>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY BUSINESS ID
        public async Task<List<User>> GetUserByBusinessId(Guid businessId)
        {
            try
            {
                var query = "SELECT * FROM [User] u LEFT JOIN Business b ON b.Id = u.BusinessId WHERE b.Id = @BusinessId";
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<User>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE BUSINESS ID FOR BUSINESS MANAGER
        public async Task<int> UpdateBusinessIdForBuM(Guid? businessId, Guid businesManagerId)
        {
            try
            {
                var query = "UPDATE [User] SET BusinessId = @BusinessId WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businesManagerId, DbType.Guid);
                parameters.Add("BusinessId", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE STATUS
        public async Task<int> UpdateUserStatus(Guid userId, string status, Guid currentUserId)
        {
            try
            {
                var query = "UPDATE [User] "
                    + "     SET "
                    + "         Status = ISNULL(@Status, Status), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Status", status, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE EMAIL
        public async Task<int> UpdateUserEmail(Guid userId, string email, Guid currentUserId)
        {
            try
            {
                var query = "UPDATE [User] "
                        + "     SET "
                        + "         Email = ISNULL(@Email, Email), "
                        + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                        + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                        + "     WHERE "
                        + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

        //DELETE BY BUSINESS
        public async Task<int> DeleteUserByBusinessId(Guid businessId)
        {
            try
            {
                var query = "DELETE FROM [User] "
                    + "     WHERE "
                    + "         BusinessId = @BusinessId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<IntegrateInfo> GetIntegrateInfoByEmailAndProjectId(string email, Guid projectId)
        {
            try
            {
                var query = "SELECT p.Id AS ProjectId, u.Id AS UserId , u.SecretKey, p.AccessKey FROM Project p JOIN [User] u ON p.ManagerId = u.Id WHERE u.Email = @Email and p.Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                parameters.Add("Id", projectId, DbType.Guid);
                return await connection.QueryFirstOrDefaultAsync<IntegrateInfo>(query, parameters);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<Guid> GetProjectIdByManagerEmail(string email)
        {
            try
            {
                var query = "SELECT p.Id FROM [User] u JOIN Project p ON u.Id = p.ManagerId WHERE u.Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Guid>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<Guid>> GetUsersIdByRoleIdAndBusinessId(Guid roleId, string businessId)
        {
            try
            {
                var query = "";
                var parameters = new DynamicParameters();
                if (businessId == null || businessId.Equals(""))
                {
                    query = "SELECT Id FROM [User] WHERE RoleId = @RoleId ";
                    parameters.Add("RoleId", roleId, DbType.Guid);
                }
                else
                {
                    query = "SELECT Id FROM [User] WHERE RoleId = @RoleId AND BusinessId = @BusinessId";
                    parameters.Add("RoleId", roleId, DbType.Guid);
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                }

                using var connection = CreateConnection();
                return (await connection.QueryAsync<Guid>(query, parameters)).ToList();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<User> GetUserByInvestorId(Guid investorId)
        {
            try
            {
                var query = "SELECT U.* FROM Investor I JOIN [User] U ON I.UserId = U.Id WHERE I.Id = @InvestorId";
                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

    }
}
