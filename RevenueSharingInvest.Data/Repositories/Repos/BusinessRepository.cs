using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class BusinessRepository : BaseRepository, IBusinessRepository
    {
        public BusinessRepository(IConfiguration configuration) : base(configuration)
        {
        }        

        //CREATE
        public async Task<string> CreateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO)
        {
            try
            {
                var query = "INSERT INTO Business (" 
                    + "         Name, " 
                    + "         PhoneNum,"    
                    + "         Image, " 
                    + "         Email, "
                    + "         Description, "
                    + "         TaxIdentificationNumber, "
                    + "         Address, "
                    + "         NumOfProject, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ("
                    + "         @Name, "
                    + "         @PhoneNum, "
                    + "         @Image, "
                    + "         @Email, "
                    + "         @Description, "
                    + "         @TaxIdentificationNumber, "
                    + "         @Address, "
                    + "         0, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy)";
                var parameters = new DynamicParameters();
                parameters.Add("Name", businessDTO.Name, DbType.String);
                parameters.Add("PhoneNum", businessDTO.PhoneNum, DbType.String);
                parameters.Add("Image", businessDTO.Image, DbType.String);
                parameters.Add("Email", businessDTO.Email, DbType.String);
                parameters.Add("Description", businessDTO.Description, DbType.String);
                parameters.Add("TaxIdentificationNumber", businessDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("Address", businessDTO.Address, DbType.String);
                parameters.Add("Status", businessDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", businessDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.CreateBy, DbType.Guid);

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
        public async Task<int> DeleteBusinessById(Guid businesssId)
        {
            try
            {
                var query = "DELETE FROM Business "
                    + "     WHERE "
                    + "         Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", businesssId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RevenueSharingInvest.Data.Models.Entities.Business>> GetAllBusiness(int pageIndex, int pageSize, string status, string name, string orderBy, string order, string roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";
                var orderByCondition = "ORDER BY CreateDate";
                var orderCondition = "";
                
                var statusCondition = " AND Status = @Status ";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";

                if (roleId.Equals(""))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }
                
                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(2) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }
                
                else if(roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (orderBy != null)
                {
                    orderByCondition = "ORDER BY " + orderBy;
                }
                if (order != null)
                {
                    orderCondition = order;
                }

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    +                   orderByCondition
                    + "                 " + orderCondition + " ) AS Num, "
                    + "             * "
                    + "         FROM Business "
                    +           whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         PhoneNum,"
                    + "         Image, "
                    + "         Email, "
                    + "         Description, "
                    + "         TaxIdentificationNumber, "
                    + "         Address, "
                    + "         NumOfProject, "
                    + "         NumOfSuccessfulProject, "
                    + "         SuccessfulRate, "
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
                    return (await connection.QueryAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Business " + whereCondition + " " + orderByCondition + " " + orderCondition;
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query, parameters)).ToList();
                }              
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessById(Guid businesssId)
        {
            try
            {
                var query = "SELECT * FROM Business WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businesssId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }        
        
        //GET BY EMAIL
        public async Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByEmail(string email)
        {
            try
            {
                var query = "SELECT * FROM Business WHERE Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO, Guid businesssId)
        {
            try
            {
                var query = "UPDATE Business "
                    + "     SET "
                    + "         Name = ISNULL(@Name, Name), "
                    + "         PhoneNum = ISNULL(@PhoneNum, PhoneNum), "
                    + "         Image = ISNULL(@Image, Image), "
                    + "         Email = ISNULL(@Email, Email), "
                    + "         Description = ISNULL(@Description, Description), "
                    + "         TaxIdentificationNumber = ISNULL(@TaxIdentificationNumber, TaxIdentificationNumber), "
                    + "         Address = ISNULL(@Address, Address), "
                    + "         UpdateDate = ISNULL(@UpdateDate, UpdateDate), "
                    + "         UpdateBy = ISNULL(@UpdateBy, UpdateBy) "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", businessDTO.Name, DbType.String);
                parameters.Add("PhoneNum", businessDTO.PhoneNum, DbType.String);
                parameters.Add("Image", businessDTO.Image, DbType.String);
                parameters.Add("Email", businessDTO.Email, DbType.String);
                parameters.Add("Description", businessDTO.Description, DbType.String);
                parameters.Add("TaxIdentificationNumber", businessDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("Address", businessDTO.Address, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", businesssId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE STATUS
        public async Task<int> UpdateBusinessStatus(Guid businessId, String status, Guid updaterId)
        {
            try
            {
                var query = "UPDATE " 
                    + "         Business " 
                    + "     SET " 
                    + "         Status = @Status, " 
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE " 
                    + "         Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Status", status, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", updaterId, DbType.Guid);
                parameters.Add("Id", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);

            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE IMAGE
        public async Task<int> UpdateBusinessImage(string url, Guid businessId)
        {
            try
            {
                var query = "UPDATE Business SET Image = @Image WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Image", url, DbType.String);
                parameters.Add("Id", businessId, DbType.Guid);
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
        public async Task<int> ClearAllBusinessData()
        {
            try
            {
                var query = "DELETE FROM Business";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE BY BUSINESSID
        public async Task<int> DeleteBusinessByBusinessId(Guid businessId)
        {
            try
            {
                var query = "DELETE FROM Busisness WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //COUNT BUSINESS
        public async Task<int> CountBusiness(string status, string name,string roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var statusCondition = " AND Status = @Status ";
                var nameCondition = " AND Name LIKE '%" + name + "%' ";

                if (roleId.Equals(""))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(0) + "' OR Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "' OR Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(2) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                else if (roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    if (status != null)
                    {
                        whereCondition = whereCondition + statusCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (Status = '"
                        + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(1) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                var query = "SELECT COUNT(*) FROM Business " + whereCondition;

                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }


        //GET BY USER ID
        public async Task<Models.Entities.Business> GetBusinessByUserId(Guid userId)
        {
            try
            {
                var query = "SELECT " 
                    + "         B.* " 
                    + "     FROM " 
                    + "         Business B " 
                    + "         JOIN [User] U ON B.Id = U.BusinessId " 
                    + "     WHERE " 
                    + "         U.Id = @UserId";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Models.Entities.Business>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY PROJECT ID
        public async Task<Models.Entities.Business> GetBusinessByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT "
                    + "         B.* "
                    + "     FROM "
                    + "         Business B "
                    + "         JOIN Project P ON B.Id = P.BusinessId "
                    + "     WHERE "
                    + "         P.Id = @ProjectId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Models.Entities.Business>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE NUM OF PROJECT
        public async Task<int> UpdateBusinessNumOfProject(Guid businessId)
        {
            try
            {
                var query = "UPDATE Business SET NumOfProject = NumOfProject + 1 WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
