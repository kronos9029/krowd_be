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
    public class RiskTypeRepository : BaseRepository, IRiskTypeRepository
    {
        public RiskTypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateRiskType(RiskType riskTypeDTO)
        {
            try
            {
                var query = "INSERT INTO RiskType ("
                    + "         Name, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", riskTypeDTO.Name, DbType.String);
                parameters.Add("Description", riskTypeDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", riskTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", riskTypeDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteRiskTypeById(Guid riskTypeId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE RiskType "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", riskTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", riskTypeId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RiskType>> GetAllRiskTypes()
        {
            try
            {
                string query = "SELECT * FROM RiskType WHERE IsDeleted = 0";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<RiskType>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<RiskType> GetRiskTypeById(Guid riskTypeId)
        {
            try
            {
                string query = "SELECT * FROM RiskType WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", riskTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RiskType>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateRiskType(RiskType riskTypeDTO, Guid riskTypeId)
        {
            try
            {
                var query = "UPDATE RiskType "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Description = @Description, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", riskTypeDTO.Name, DbType.String);
                parameters.Add("Description", riskTypeDTO.Description, DbType.String);
                parameters.Add("CreateDate", riskTypeDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", riskTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", riskTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", riskTypeDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", riskTypeId, DbType.Guid);

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
