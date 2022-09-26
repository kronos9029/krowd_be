using Dapper;
using Microsoft.Extensions.Configuration;
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
    public class RiskRepository : BaseRepository, IRiskRepository
    {
        public RiskRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateRisk(Risk riskDTO)
        {
            try
            {
                var query = "INSERT INTO Risk ("
                    + "         Name, "
                    + "         ProjectId, "
                    + "         RiskTypeId, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @ProjectId, "
                    + "         @RiskTypeId, "
                    + "         @Description, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", riskDTO.Name, DbType.String);
                parameters.Add("ProjectId", riskDTO.ProjectId, DbType.Guid);
                parameters.Add("RiskTypeId", riskDTO.RiskTypeId, DbType.Guid);
                parameters.Add("Description", riskDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", riskDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", riskDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteRiskById(Guid riskId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE Risk "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", riskDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", riskId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Risk>> GetAllRisks(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     RiskTypeId, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Risk "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         ProjectId, "
                    + "         RiskTypeId, "
                    + "         Description, "
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
                    return (await connection.QueryAsync<Risk>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Risk WHERE IsDeleted = 0 ORDER BY RiskTypeId, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Risk>(query)).ToList();
                }                
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }        
        
        public async Task<List<Risk>> GetAllRisksByBusinessId(int pageIndex, int pageSize, Guid businessId)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     RiskTypeId, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Risk "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         x.Id, "
                    + "         x.Name, "
                    + "         x.ProjectId, "
                    + "         x.RiskTypeId, "
                    + "         x.Description, "
                    + "         x.CreateDate, "
                    + "         x.CreateBy, "
                    + "         x.UpdateDate, "
                    + "         x.UpdateBy, "
                    + "         x.IsDeleted "
                    + "     FROM "
                    + "         X x JOIN Project p ON x.ProjectId = p.Id"
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize AND p.BusinessId = @BusinessId";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Risk>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT " +
                        "r.Id, " +
                        "r.Name, " +
                        "r.ProjectId, " +
                        "r.RiskTypeId, " +
                        "r.Description, " +
                        "r.CreateDate, " +
                        "r.CreateBy, " +
                        "r.UpdateDate, " +
                        "r.UpdateBy, " +
                        "r.IsDeleted " +
                        "FROM Risk r JOIN Project p ON r.ProjectId = p.Id " +
                        "WHERE r.IsDeleted = 0 AND p.BusinessId = @BusinessId " +
                        "ORDER BY RiskTypeId, r.Name ASC";
                    var parameters = new DynamicParameters();
                    parameters.Add("BusinessId", businessId, DbType.Guid);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Risk>(query, parameters)).ToList();
                }                
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Risk> GetRiskById(Guid riskId)
        {
            try
            {
                string query = "SELECT * FROM Risk WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Risk>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }        

        public async Task<string> GetBusinessByRiskId(Guid riskId)
        {
            try
            {
                var query = "SELECT DISTINCT p.BusinessId FROM Risk r JOIN Project p ON r.ProjectId = p.Id WHERE r.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskId, DbType.Guid);
                using var connection = CreateConnection();
                return connection.QueryFirstOrDefaultAsync<Guid>(query, parameters).ToString();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateRisk(Risk riskDTO, Guid riskId)
        {
            try
            {
                var query = "UPDATE Risk "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         ProjectId = @ProjectId, "
                    + "         RiskTypeId = @RiskTypeId, "
                    + "         Description = @Description, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", riskDTO.Name, DbType.String);
                parameters.Add("ProjectId", riskDTO.ProjectId, DbType.Guid);
                parameters.Add("RiskTypeId", riskDTO.RiskTypeId, DbType.Guid);
                parameters.Add("Description", riskDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", riskDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", riskDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", riskId, DbType.Guid);

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
        public async Task<int> ClearAllRiskData()
        {
            try
            {
                var query = "DELETE FROM Risk";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
