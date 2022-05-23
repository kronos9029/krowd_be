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
    public class AreaRepository : BaseRepository, IAreaRepository
    {
        public AreaRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateArea(Area areaDTO)
        {
            try
            {
                var query = "INSERT INTO Area ("
                    + "         City, " 
                    + "         District, " 
                    + "         Ward, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @City, "
                    + "         @District, "
                    + "         @Ward, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("City", areaDTO.City, DbType.String);
                parameters.Add("District", areaDTO.District, DbType.String);
                parameters.Add("Ward", areaDTO.Ward, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", areaDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteAreaById(Guid areaId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE Area " 
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
                parameters.Add("Id", areaId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Area>> GetAllAreas()
        {
            try
            {
                string query = "SELECT * FROM Area WHERE IsDeleted = 0";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Area>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Area> GetAreaById(Guid areaId)
        {
            try
            {
                string query = "SELECT * FROM Area WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", areaId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Area>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateArea(Area areaDTO, Guid areaId)
        {
            try
            {
                var query = "UPDATE Area "
                    + "     SET "
                    + "         City = @City, "
                    + "         District = @District, "
                    + "         Ward = @Ward, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("City", areaDTO.City, DbType.String);
                parameters.Add("District", areaDTO.District, DbType.String);
                parameters.Add("Ward", areaDTO.Ward, DbType.String);
                parameters.Add("CreateDate", areaDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", areaDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", areaDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", areaId, DbType.Guid);

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
