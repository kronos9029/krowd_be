using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
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
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
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
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("RoleId", userDTO.RoleId, DbType.Guid);
                parameters.Add("BusinessId", userDTO.BusinessId, DbType.Guid);
                parameters.Add("LastName", userDTO.LastName, DbType.String);
                parameters.Add("FirstName", userDTO.FirstName, DbType.String);
                parameters.Add("Image", userDTO.Image, DbType.String);
                parameters.Add("Email", userDTO.Email, DbType.String);
                parameters.Add("Status", userDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", userDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", userDTO.CreateBy, DbType.Guid);

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
        public async Task<int> DeleteUserById(Guid userId)//thiếu para UpdateBy đợi Authen
        {
            try
            {
                var query = "UPDATE [User] "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", userDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<User>> GetAllUsers(int pageIndex, int pageSize, string businessId, string projectManagerId, string roleId, string status, string thisUserRoleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var selectCondition = " * ";
                var fromCondition = " [User] ";
                var whereCondition = "";
                var groupByCondition = " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                var isDeletedCondition = " AND IsDeleted = 0 ";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var roleIdCondition = " AND RoleId = @RoleId ";
                var statusCondition = " AND Status = @Status ";

                if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
                    }

                    if (roleId != null)
                    {
                        if (businessId != null)
                        {
                            if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                            {
                                selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                                fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                                whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
                                parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                                if (status != null)
                                {
                                    whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                    parameters.Add("Status", status, DbType.String);
                                }
                                else
                                {
                                    whereCondition = whereCondition + " AND (U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                                    + groupByCondition;
                                }
                            }
                            else
                            {
                                whereCondition = whereCondition + roleIdCondition;
                                parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                        + isDeletedCondition;
                    }

                    if (roleId != null)
                    {
                        if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                        {
                            selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                            fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                            whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                            if (status != null)
                            {
                                whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                parameters.Add("Status", status, DbType.String);
                            }
                            else
                            {
                                whereCondition = whereCondition + " AND (U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') AND U.IsDeleted = 0 "
                                + groupByCondition;
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }                       
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                        + isDeletedCondition;
                    }

                    if (roleId != null)
                    {
                        if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                        {
                            selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                            fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                            whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE ManagerId = @ManagerId) " + roleIdCondition;
                            parameters.Add("ManagerId", Guid.Parse(projectManagerId), DbType.Guid);
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                            if (status != null)
                            {
                                whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                parameters.Add("Status", status, DbType.String);
                            }
                            else
                            {
                                whereCondition = whereCondition + " AND (U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') AND U.IsDeleted = 0 "
                                + groupByCondition;
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     RoleId, "
                    + "                     FirstName ASC ) AS Num, "
                    +               selectCondition
                    + "         FROM " + fromCondition
                    +           whereCondition + " ) "
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
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT "+ selectCondition + " FROM " + fromCondition + " " + whereCondition + " ORDER BY RoleId, FirstName ASC";
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
                            "            U.UpdateBy, " +
                            "            U.IsDeleted " +
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
                            "         U.UpdateBy, " +
                            "         U.IsDeleted " +
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
                            "FROM(SELECT U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted "+
                            "FROM[User] U "+
                            "    JOIN Investor INS ON U.Id = INS.UserId "+
                            "    JOIN Investment INM ON INS.Id = INM.InvestorId "+
                            "   WHERE INM.ProjectId IN(SELECT Id "+
                            "                        FROM Project "+
                            "                        WHERE ManagerId = @ManagerId) "+
                            " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted "+
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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
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

        //CLEAR DATA
        public async Task<int> ClearAllUserData()
        {
            try
            {
                var query = "DELETE FROM [User]";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
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

        public async Task<int> CountUser(string businessId, string projectManagerId, string roleId, string status, string thisUserRoleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var selectCondition = " * ";
                var fromCondition = " [User] ";
                var whereCondition = "";
                var groupByCondition = " GROUP BY U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                var isDeletedCondition = " AND IsDeleted = 0 ";

                var businessIdCondition = " AND BusinessId = @BusinessId ";
                var roleIdCondition = " AND RoleId = @RoleId ";
                var statusCondition = " AND Status = @Status ";

                if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (businessId != null)
                    {
                        whereCondition = whereCondition + businessIdCondition;
                        parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') ";
                    }

                    if (roleId != null)
                    {
                        if (businessId != null)
                        {
                            if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                            {
                                selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                                fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                                whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
                                parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                                if (status != null)
                                {
                                    whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                    parameters.Add("Status", status, DbType.String);
                                }
                                else
                                {
                                    whereCondition = whereCondition + " AND (U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                    + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                                    + groupByCondition;
                                }
                            }
                            else
                            {
                                whereCondition = whereCondition + roleIdCondition;
                                parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                        + isDeletedCondition;
                    }

                    if (roleId != null)
                    {
                        if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                        {
                            selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                            fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                            whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) " + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                            if (status != null)
                            {
                                whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                parameters.Add("Status", status, DbType.String);
                            }
                            else
                            {
                                whereCondition = whereCondition + " AND (U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') AND U.IsDeleted = 0 "
                                + groupByCondition;
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if(thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') "
                        + isDeletedCondition;
                    }

                    if (roleId != null)
                    {
                        if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                        {
                            selectCondition = " U.Id, U.BusinessId, U.RoleId, U.Description, U.LastName, U.FirstName, U.PhoneNum, U.Image, U.IdCard, U.Email, U.Gender, U.DateOfBirth, U.TaxIdentificationNumber, U.City, U.District, U.Address, U.BankName, U.BankAccount, U.Status, U.CreateDate, U.CreateBy, U.UpdateDate, U.UpdateBy, U.IsDeleted ";
                            fromCondition = " [User] U JOIN Investor INS ON U.Id = INS.UserId JOIN Investment INM ON INS.Id = INM.InvestorId ";
                            whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE ManagerId = @ManagerId) " + roleIdCondition;
                            parameters.Add("ManagerId", Guid.Parse(projectManagerId), DbType.Guid);
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);

                            if (status != null)
                            {
                                whereCondition = whereCondition + " AND U.Status = @Status" + groupByCondition;
                                parameters.Add("Status", status, DbType.String);
                            }
                            else
                            {
                                whereCondition = whereCondition + " AND (U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR U.Status = '"
                                + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') AND U.IsDeleted = 0 "
                                + groupByCondition;
                            }
                        }
                        else
                        {
                            whereCondition = whereCondition + roleIdCondition;
                            parameters.Add("RoleId", Guid.Parse(roleId), DbType.Guid);
                        }
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                var query = "SELECT COUNT(*) FROM (SELECT " + selectCondition + " FROM " + fromCondition + " " + whereCondition + ") AS X";

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);
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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

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
    }
}
