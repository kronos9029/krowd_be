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
    public class PeriodRevenueRepository : BaseRepository, IPeriodRevenueRepository
    {
        public PeriodRevenueRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreatePeriodRevenue(PeriodRevenue periodRevenueDTO)
        {
            try
            {
                var query = "INSERT INTO PeriodRevenue ("
                    + "         ProjectId, "
                    + "         StageId, "
                    + "         ActualAmount, "
                    + "         PessimisticExpectedAmount, "
                    + "         NormalExpectedAmount, "
                    + "         OptimisticExpectedAmount, "
                    + "         PessimisticExpectedRatio, "
                    + "         NormalExpectedRatio, "
                    + "         OptimisticExpectedRatio, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @StageId, "
                    + "         @ActualAmount, "
                    + "         @PessimisticExpectedAmount, "
                    + "         @NormalExpectedAmount, "
                    + "         @OptimisticExpectedAmount, "
                    + "         @PessimisticExpectedRatio, "
                    + "         @NormalExpectedRatio, "
                    + "         @OptimisticExpectedRatio, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", periodRevenueDTO.ProjectId, DbType.Guid);
                parameters.Add("StageId", periodRevenueDTO.StageId, DbType.Guid);
                parameters.Add("ActualAmount", periodRevenueDTO.ActualAmount, DbType.Double);
                parameters.Add("PessimisticExpectedAmount", periodRevenueDTO.PessimisticExpectedAmount, DbType.Double);
                parameters.Add("NormalExpectedAmount", periodRevenueDTO.NormalExpectedAmount, DbType.Double);
                parameters.Add("OptimisticExpectedAmount", periodRevenueDTO.OptimisticExpectedAmount, DbType.Double);
                parameters.Add("PessimisticExpectedRatio", periodRevenueDTO.PessimisticExpectedRatio, DbType.Double);
                parameters.Add("NormalExpectedRatio", periodRevenueDTO.NormalExpectedRatio, DbType.Double);
                parameters.Add("OptimisticExpectedRatio", periodRevenueDTO.OptimisticExpectedRatio, DbType.Double);
                parameters.Add("Status", periodRevenueDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePeriodRevenueById(Guid periodRevenueId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE PeriodRevenue "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", periodRevenueDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", periodRevenueId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenue>> GetAllPeriodRevenues()
        {
            try
            {
                string query = "SELECT * FROM PeriodRevenue";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<PeriodRevenue>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<PeriodRevenue> GetPeriodRevenueById(Guid periodRevenueId)
        {
            try
            {
                string query = "SELECT * FROM PeriodRevenue WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", periodRevenueId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<PeriodRevenue>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenue(PeriodRevenue periodRevenueDTO, Guid periodRevenueId)
        {
            try
            {
                var query = "UPDATE PeriodRevenue "
                    + "     SET "
                    + "         ProjectId = @ProjectId, "
                    + "         StageId = @StageId, "
                    + "         ActualAmount = @ActualAmount, "
                    + "         PessimisticExpectedAmount = @PessimisticExpectedAmount, "
                    + "         NormalExpectedAmount = @NormalExpectedAmount, "
                    + "         OptimisticExpectedAmount = @OptimisticExpectedAmount, "
                    + "         PessimisticExpectedRatio = @PessimisticExpectedRatio, "
                    + "         NormalExpectedRatio = @NormalExpectedRatio, "
                    + "         OptimisticExpectedRatio = @OptimisticExpectedRatio, "
                    + "         Status = @Status, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", periodRevenueDTO.ProjectId, DbType.Guid);
                parameters.Add("StageId", periodRevenueDTO.StageId, DbType.Guid);
                parameters.Add("ActualAmount", periodRevenueDTO.ActualAmount, DbType.Double);
                parameters.Add("PessimisticExpectedAmount", periodRevenueDTO.PessimisticExpectedAmount, DbType.Double);
                parameters.Add("NormalExpectedAmount", periodRevenueDTO.NormalExpectedAmount, DbType.Double);
                parameters.Add("OptimisticExpectedAmount", periodRevenueDTO.OptimisticExpectedAmount, DbType.Double);
                parameters.Add("PessimisticExpectedRatio", periodRevenueDTO.PessimisticExpectedRatio, DbType.Double);
                parameters.Add("NormalExpectedRatio", periodRevenueDTO.NormalExpectedRatio, DbType.Double);
                parameters.Add("OptimisticExpectedRatio", periodRevenueDTO.OptimisticExpectedRatio, DbType.Double);
                parameters.Add("Status", periodRevenueDTO.Status, DbType.String);
                parameters.Add("CreateDate", periodRevenueDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", periodRevenueDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", periodRevenueId, DbType.Guid);

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
