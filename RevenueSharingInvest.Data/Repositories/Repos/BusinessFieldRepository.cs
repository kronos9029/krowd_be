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
    public class BusinessFieldRepository : BaseRepository, IBusinessFieldRepository
    {
        public BusinessFieldRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateBusinessField(Guid businessId, Guid fieldId, Guid creatorId)
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
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("FieldId", fieldId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", creatorId, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", creatorId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
        
        //DELETE
        public async Task<int> DeleteBusinessFieldById(Guid businessId, Guid fieldId)
        {
            try
            {
                var query = "UPDATE BusinessField "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         BusinessId=@BusinessId "
                    + "         AND FieldId=@FieldId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("FieldId", fieldId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<BusinessField>> GetAllBusinessFields(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     FieldId ASC ) AS Num, "
                    + "             * "
                    + "         FROM BusinessField "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         BusinessId, "
                    + "         FieldId, "
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
                    return (await connection.QueryAsync<BusinessField>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM BusinessField WHERE IsDeleted = 0 ORDER BY FieldId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<BusinessField>(query)).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<BusinessField> GetBusinessFieldById(Guid businessId, Guid fieldId)
        {
            try
            {
                string query = "SELECT * FROM BusinessField WHERE BusinessId = @BusinessId AND FieldId = @FieldId";
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                parameters.Add("FieldId", fieldId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<BusinessField>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusinessField(BusinessField businessFieldDTO, Guid businessId, Guid fieldId)
        {
            try
            {
                var query = "UPDATE BusinessField "
                    + "     SET "
                    + "         BusinessId = @BusinessId, "
                    + "         FieldId = @FieldId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         BusinessId=@BId "
                    + "         AND FieldId=@FId";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessFieldDTO.BusinessId, DbType.Guid);
                parameters.Add("FieldId", businessFieldDTO.FieldId, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessFieldDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", businessFieldDTO.IsDeleted, DbType.Boolean);
                parameters.Add("BId", businessId, DbType.Guid);
                parameters.Add("FId", fieldId, DbType.Guid);

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

        public async Task<int> ClearAllBusinessFieldData()
        {
            try
            {
                var query = "DELETE FROM BusisnessField";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> DeleteBusinessFieldByBusinessId(Guid businessId)
        {
            try
            {
                var query = "DELETE FROM BusinessField WHERE BusinessId = @BusinessId";
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
