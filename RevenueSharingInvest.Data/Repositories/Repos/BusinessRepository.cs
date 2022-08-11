using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.Constants;
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
                    + "         NumOfSuccessfulProject, "
                    + "         SuccessfulRate, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
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
                    + "         null, "
                    + "         null, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0)";
                var parameters = new DynamicParameters();
                parameters.Add("Name", businessDTO.Name, DbType.String);
                parameters.Add("PhoneNum", businessDTO.PhoneNum, DbType.String);
                parameters.Add("Image", businessDTO.Image, DbType.String);
                parameters.Add("Email", businessDTO.Email, DbType.String);
                parameters.Add("Description", businessDTO.Description, DbType.String);
                parameters.Add("TaxIdentificationNumber", businessDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("Address", businessDTO.Address, DbType.String);
                parameters.Add("Status", businessDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", businessDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteBusinessById(Guid businesssId)
        {
            try
            {
                var query = "UPDATE Business "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", businesssId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RevenueSharingInvest.Data.Models.Entities.Business>> GetAllBusiness(int pageIndex, int pageSize, string? orderBy, string? order, string role)
        {
            try
            {
                var whereCondition = "";
                var orderByCondition = "ORDER BY CreateDate";
                var orderCondition = "";

                if (role.Equals("ADMIN"))
                {
                    whereCondition = "";
                }
                if (role.Equals("INVESTOR"))
                {
                    whereCondition = "WHERE IsDeleted = 0 AND Status = " + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0);
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
                    return (await connection.QueryAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Business " + whereCondition + " " + orderByCondition + " " + orderCondition;
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<RevenueSharingInvest.Data.Models.Entities.Business>(query)).ToList();
                }              
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

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
                throw new Exception(e.Message, e);
            }
        }        
        
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
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE - chưa update NumOfProject, NumOfSuccessfulProject, SuccessfulRate (update riêng)
        public async Task<int> UpdateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO, Guid businesssId)
        {
            try
            {
                var query = "UPDATE Business "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         PhoneNum = @PhoneNum, "
                    + "         Image = @Image, "
                    + "         Email = @Email, "
                    + "         Description = @Description, "
                    + "         TaxIdentificationNumber = @TaxIdentificationNumber, "
                    + "         Address = @Address, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", businessDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", businesssId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> UpdateBusinessStatus(Guid businessId, String status)
        {
            try
            {
                var query = "UPDATE Business SET Status = @Status WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Status", status, DbType.String);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);

            } catch(Exception e)
            {
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
                throw new Exception(e.Message);
            }
        }

        //DELETE BY BUSINESSID
        public async void DeleteBusinessByBusinessId(Guid businessId)
        {
            try
            {
                var query = "DELETE FROM Busisness WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businessId, DbType.Guid);
                using var connection = CreateConnection();
                await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> CountBusiness(string role)
        {
            try
            {
                var whereCondition = "";
                
                if (role.Equals("ADMIN"))
                {
                    whereCondition = "";
                }
                if (role.Equals("INVESTOR"))
                {
                    whereCondition = "WHERE IsDeleted = 0 AND Status = " + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0);
                }            

                var query = "SELECT COUNT(*) FROM Business " + whereCondition;

                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

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
                throw new Exception(e.Message, e);
            }
        }

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
                throw new Exception(e.Message, e);
            }
        }
    }
}
