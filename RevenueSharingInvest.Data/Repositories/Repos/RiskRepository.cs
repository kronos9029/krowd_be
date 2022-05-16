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
    public class RiskRepository : BaseRepository, IRiskRepository
    {
        public RiskRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateRisk(Risk riskDTO)
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
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
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
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Risk>> GetAllRisks()
        {
            try
            {
                string query = "SELECT * FROM Risk";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Risk>(query)).ToList();
            }
            catch (Exception e)
            {
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
                throw new Exception(e.Message, e);
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
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
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
                parameters.Add("CreateDate", riskDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", riskDTO.CreateBy, DbType.Guid);
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
                throw new Exception(e.Message, e);
            }
        }
    }
}
