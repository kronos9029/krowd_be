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
    public class BusinessFieldRepository : BaseRepository, IBusinessFieldRepository
    {
        public BusinessFieldRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateBusinessField(BusinessField businessFieldDTO)
        {
            try
            {
                var query = "INSERT INTO BusinessField ("
                    + "         BusinessId, "
                    + "         FieldId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @BusinessId, "
                    + "         @FieldId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessFieldDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", businessFieldDTO.FieldId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", businessFieldDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessFieldDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        
        //DELETE
        public async Task<int> DeleteBusinessFieldById(Guid businessFieldId)
        {
            try
            {
                var query = "UPDATE BusinessField "
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
                parameters.Add("Id", businessFieldId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<BusinessField>> GetAllBusinessFields()
        {
            try
            {
                string query = "SELECT * FROM BusinessField";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<BusinessField>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusinessField(BusinessField businessFieldDTO, Guid businessFieldId)
        {
            try
            {
                var query = "UPDATE BusinessField "
                    + "     SET "
                    + "         BusinessId = @BusinessId, "
                    + "         FieldId = @FieldId, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessFieldDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", businessFieldDTO.FieldId, DbType.Guid);
                parameters.Add("CreateDate", businessFieldDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", businessFieldDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessFieldDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", businessFieldDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", businessFieldId, DbType.Guid);

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
