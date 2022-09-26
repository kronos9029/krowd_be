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
    public class PeriodRevenueHistoryRepository : BaseRepository, IPeriodRevenueHistoryRepository
    {
        public PeriodRevenueHistoryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO)
        {
            try
            {
                var query = "INSERT INTO PeriodRevenueHistory ("
                    + "         Name, "
                    + "         PeriodRevenueId, "
                    + "         Description, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @PeriodRevenueId, "
                    + "         @Description, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", periodRevenueHistoryDTO.Name, DbType.String);
                parameters.Add("PeriodRevenueId", periodRevenueHistoryDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Description", periodRevenueHistoryDTO.Description, DbType.String);
                parameters.Add("Status", periodRevenueHistoryDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueHistoryDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueHistoryDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePeriodRevenueHistoryByProjectId(Guid projectIdId)
        {
            try
            {
                var query = "DELETE FROM PeriodRevenueHistory "
                    + "     WHERE "
                    + "         Id IN  "
                    + "         (SELECT " 
                    + "             PH.Id " 
                    + "         FROM " 
                    + "             PeriodRevenueHistory PH " 
                    + "             JOIN PeriodRevenue P ON PH.PeriodRevenueId = P.Id " 
                    + "         WHERE " 
                    + "             P.ProjectId = @ProjectId)";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectIdId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueHistory>> GetAllPeriodRevenueHistories(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     PeriodRevenueId, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM PeriodRevenueHistory "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         PeriodRevenueId, "
                    + "         Description, "
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
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query, parameters)).ToList();
                }

                else
                {
                    var query = "SELECT * FROM PeriodRevenueHistory WHERE IsDeleted = 0 ORDER BY PeriodRevenueId, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenueHistory>(query)).ToList();
                }              
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<PeriodRevenueHistory> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId)
        {
            try
            {
                string query = "SELECT * FROM PeriodRevenueHistory WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", periodRevenueHistoryId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<PeriodRevenueHistory>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistory periodRevenueHistoryDTO, Guid periodRevenueHistoryId)
        {
            try
            {
                var query = "UPDATE PeriodRevenueHistory "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         PeriodRevenueId = @PeriodRevenueId, "
                    + "         Description = @Description, "
                    + "         Status = @Status, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", periodRevenueHistoryDTO.Name, DbType.String);
                parameters.Add("PeriodRevenueId", periodRevenueHistoryDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Description", periodRevenueHistoryDTO.Description, DbType.String);
                parameters.Add("Status", periodRevenueHistoryDTO.Status, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueHistoryDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", periodRevenueHistoryDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", periodRevenueHistoryId, DbType.Guid);

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
        public async Task<int> ClearAllPeriodRevenueHistoryData()
        {
            try
            {
                var query = "DELETE FROM PeriodRevenueHistory";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
