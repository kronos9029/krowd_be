using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
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
    public class InvestorRepository : BaseRepository, IInvestorRepository
    {
        public InvestorRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateInvestor(Investor investorDTO)
        {
            try
            {
                var query = "INSERT INTO Investor ("
                    + "         UserId, "
//                    + "         InvestorTypeId, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @UserId, "
//                    + "         @InvestorTypeId, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", investorDTO.UserId, DbType.Guid);
                parameters.Add("InvestorTypeId", Guid.Parse(InvestorTypeDictionary.investorType.GetValueOrDefault("Nhà đầu tư ngắn hạn")), DbType.Guid);
                parameters.Add("Status", ObjectStatusEnum.ACTIVE.ToString(), DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investorDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investorDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteInvestorById(Guid investorId)
        {
            try
            {
                var query = "UPDATE Investor "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", investorDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Investor>> GetAllInvestors(
            int pageIndex,
            int pageSize,
            string businessId,
            string projectManagerId,
            string status,
            string name,
            string orderBy,
            string order,
            string thisUserRoleId)
        {
            var parameters = new DynamicParameters();

            var selectCondition = " * ";
            var fromCondition = " Investor ";
            var whereCondition = "";
            var groupByCondition = " GROUP BY INS.Id, INS.UserId, INS.InvestorTypeId, INS.Status, INS.CreateDate, INS.CreateBy, INS.UpdateDate, INS.UpdateBy, INS.IsDeleted ";
            var orderByCondition = "ORDER BY CreateDate";
            var orderCondition = "";
            var isDeletedCondition = " AND IsDeleted = 0 ";

            var statusCondition = " AND Status = @Status ";
            var nameCondition = " AND Name LIKE '%" + name + "%' ";

            try
            {
                if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
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
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "')";
                    }

                    if (name != null)
                    {
                        whereCondition = whereCondition + nameCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4) + " " + isDeletedCondition;
                }
                else if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    selectCondition = " INS.Id, INS.UserId, INS.InvestorTypeId, INS.Status, INS.CreateDate, INS.CreateBy, INS.UpdateDate, INS.UpdateBy, INS.IsDeleted ";
                    fromCondition = " Investor INS JOIN Investment INM ON INS.Id = INM.InvestorId ";
                    whereCondition = " AND INM.ProjectId IN (SELECT Id FROM Project WHERE BusinessId = @BusinessId) ";
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);

                    if (name != null)
                    {
                        whereCondition = whereCondition + " AND Name LIKE '%" + name + "%' ";
                    }
                    if (status != null)
                    {
                        whereCondition = whereCondition + " AND INS.Status = @Status" + groupByCondition;
                        parameters.Add("Status", status, DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND (INS.Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0) + "' OR INS.Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1) + "' OR INS.Status = '"
                        + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(2) + "') AND INS.IsDeleted = 0 "
                        + groupByCondition;
                    }

                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }
                else if (thisUserRoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {

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
                    + "                 ORDER BY "
                    + "                     InvestorTypeId ASC ) AS Num, "
                    + "             * "
                    + "         FROM Investor "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         UserId, "
                    + "         InvestorTypeId, "
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
                    return (await connection.QueryAsync<Investor>(query, parameters)).ToList();
                }                
                else
                {
                    var query = "SELECT * FROM Investor WHERE IsDeleted = 0 ORDER BY InvestorTypeId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Investor>(query)).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Investor> GetInvestorById(Guid investorId)
        {
            try
            {
                string query = "SELECT * FROM Investor WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Investor>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }        
        
        //GET BY EMAIL
        public async Task<Guid> GetInvestorByEmail(string email)
        {
            try
            {
                string query = "SELECT I.Id FROM Investor I LEFT JOIN [User] U ON I.UserId = U.Id WHERE U.Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Guid>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestor(Investor investorDTO, Guid investorId)
        {
            try
            {
                var query = "UPDATE Investor "
                    + "     SET "
                    + "         UserId = @UserId, "
                    + "         InvestorTypeId = @InvestorTypeId, "
                    + "         Status = @Status, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", investorDTO.UserId, DbType.Guid);
                parameters.Add("InvestorTypeId", investorDTO.InvestorTypeId, DbType.Guid);
                parameters.Add("Status", investorDTO.Status, DbType.Int16);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investorDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", investorDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", investorId, DbType.Guid);

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
        public async Task<int> ClearAllInvestorData()
        {
            try
            {
                var query = "DELETE FROM Investor";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Investor> GetInvestorByUserId(Guid userId)
        {
            try
            {
                string query = "SELECT * FROM Investor WHERE UserId = @UserId";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Investor>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE STATUS
        public async Task<int> UpdateInvestorStatus(Guid userId, string status, Guid currentUserId)
        {
            try
            {
                var query = "UPDATE Investor "
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
                throw new Exception(e.Message, e);
            }
        }
    }

        //public async Task<int> CreateInvestor(Investor investor)
        //{
        //    try
        //    {
        //        var query = "INSERT INTO Investor (Id, UserId, CreateDate, InvestorTypeId) VALUES (@Id, @UserId, @CreateDate, @InvestorTypeId)";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Id", investor.Id, DbType.Guid);
        //        parameters.Add("UserId", investor.UserId, DbType.Guid);
        //        parameters.Add("CreateDate", investor.CreateDate, DbType.DateTime);
        //        parameters.Add("InvestorTypeId", investor.InvestorTypeId, DbType.Guid);

        //        using (var connection = CreateConnection())
        //        {
        //            return await connection.ExecuteAsync(query, parameters);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}

        //public async Task<int> CreateInvestorType(InvestorType investorType)
        //{
        //    try
        //    {
        //        var query = "INSERT INTO InvestorType (Name, Description, CreateDate, CreateBy) VALUES (@Name, @Description, @CreateDate, @CreateBy)";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Name", investorType.Name, DbType.String);
        //        parameters.Add("Description", investorType.Description, DbType.String);
        //        parameters.Add("CreateDate", investorType.CreateDate, DbType.DateTime);
        //        parameters.Add("CreateBy", investorType.CreateBy, DbType.String);

        //        using (var connection = CreateConnection())
        //        {
        //            return await connection.ExecuteAsync(query, parameters);
        //        }
        //    } catch(Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }

        //}

        //public async Task<List<InvestorType>> GetAllInvestorType()
        //{
        //    try
        //    {
        //        string query = "SELECT * FROM InvestorType";
        //        using var connection = CreateConnection();
        //        return (await connection.QueryAsync<InvestorType>(query)).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}
    //}
}
