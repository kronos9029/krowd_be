﻿using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
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
    public class PeriodRevenueRepository : BaseRepository, IPeriodRevenueRepository
    {
        public PeriodRevenueRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreatePeriodRevenue(PeriodRevenue periodRevenueDTO)
        {
            try
            {
                var query = "INSERT INTO PeriodRevenue ("
                    + "         ProjectId, "
                    + "         StageId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @StageId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", periodRevenueDTO.ProjectId, DbType.Guid);
                parameters.Add("StageId", periodRevenueDTO.StageId, DbType.Guid);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.CreateBy, DbType.Guid);

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
        public async Task<int> DeletePeriodRevenueByStageId(Guid stageId)
        {
            try
            {
                var query = "DELETE FROM PeriodRevenue WHERE StageId = @StageId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("StageId", stageId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenue>> GetAllPeriodRevenues(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     ProjectId, "
                    + "                     StageId ASC ) AS Num, "
                    + "             * "
                    + "         FROM PeriodRevenue "
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         ProjectId, "
                    + "         StageId, "
                    + "         ActualAmount, "
                    + "         SharedAmount, "
                    + "         PaidAmount, "
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
                    + "         UpdateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenue>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM PeriodRevenue ORDER BY ProjectId, StageId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PeriodRevenue>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenue(PeriodRevenue periodRevenueDTO)
        {
            try
            {
                var query = "UPDATE PeriodRevenue "
                    + "     SET "
                    + "         ActualAmount = ISNULL(ROUND(@ActualAmount, 0), ActualAmount), "
                    + "         SharedAmount = ISNULL(ROUND(@SharedAmount, 0), SharedAmount), "
                    + "         UpdateDate = @UpdateDate "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ActualAmount", periodRevenueDTO.ActualAmount, DbType.Double);
                parameters.Add("SharedAmount", periodRevenueDTO.SharedAmount, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("Id", periodRevenueDTO.Id, DbType.Guid);

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

        //GET BY STAGE ID
        public async Task<PeriodRevenue> GetPeriodRevenueByStageId(Guid stageId)
        {
            try
            {
                string query = "SELECT * FROM PeriodRevenue WHERE StageId = @StageId";
                var parameters = new DynamicParameters();
                parameters.Add("StageId", stageId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<PeriodRevenue>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE BY STAGE ID
        public async Task<int> UpdatePeriodRevenueByStageId(PeriodRevenue periodRevenueDTO, Guid stageId)
        {
            try
            {
                var query = "UPDATE PeriodRevenue "
                    + "     SET "
                    + "         PessimisticExpectedAmount = @PessimisticExpectedAmount, "
                    + "         NormalExpectedAmount = @NormalExpectedAmount, "
                    + "         OptimisticExpectedAmount = @OptimisticExpectedAmount, "
                    + "         PessimisticExpectedRatio = @PessimisticExpectedRatio, "
                    + "         NormalExpectedRatio = @NormalExpectedRatio, "
                    + "         OptimisticExpectedRatio = @OptimisticExpectedRatio, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         StageId = @StageId";

                var parameters = new DynamicParameters();
                parameters.Add("ActualAmount", periodRevenueDTO.ActualAmount, DbType.Double);
                parameters.Add("PessimisticExpectedAmount", periodRevenueDTO.PessimisticExpectedAmount, DbType.Double);
                parameters.Add("NormalExpectedAmount", periodRevenueDTO.NormalExpectedAmount, DbType.Double);
                parameters.Add("OptimisticExpectedAmount", periodRevenueDTO.OptimisticExpectedAmount, DbType.Double);
                parameters.Add("PessimisticExpectedRatio", periodRevenueDTO.PessimisticExpectedRatio, DbType.Double);
                parameters.Add("NormalExpectedRatio", periodRevenueDTO.NormalExpectedRatio, DbType.Double);
                parameters.Add("OptimisticExpectedRatio", periodRevenueDTO.OptimisticExpectedRatio, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.UpdateBy, DbType.Guid);
                parameters.Add("StageId", stageId, DbType.Guid);

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

        //DELETE ALL BY PROJECT ID
        public async Task<int> DeletePeriodRevenueByProjectId(Guid projectId)
        {
            try
            {
                var query = "DELETE FROM PeriodRevenue WHERE ProjectId = @ProjectId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE PAID AMOUNT
        public async Task<int> UpdatePeriodRevenueByPaidAmount(PeriodRevenue periodRevenueDTO)
        {
            try
            {
                var query = "UPDATE PeriodRevenue "
                    + "     SET "
                    + "         PaidAmount = CASE " 
                    + "                         WHEN PaidAmount IS NULL THEN 0 + @PaidAmount " 
                    + "                         WHEN PaidAmount IS NOT NULL THEN PaidAmount + @PaidAmount " 
                    + "                      END, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("PaidAmount", periodRevenueDTO.PaidAmount, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", periodRevenueDTO.Id, DbType.Guid);

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

        //COUNT NOT PAID ENOUGH
        public async Task<int> CountNotPaidEnoughPeriodRevenue(Guid projectId)
        {
            try
            {
                var query = "SELECT COUNT(*) FROM PeriodRevenue WHERE ProjectId = @ProjectId AND (Status IS NULL OR Status = 'NOT_PAID_ENOUGH') ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //CREATE REPAYMENT PERIOD REVENUE
        public async Task<string> CreateRepaymentPeriodRevenue(PeriodRevenue periodRevenueDTO)
        {
            try
            {
                var query = "INSERT INTO PeriodRevenue ("
                    + "         ProjectId, "
                    + "         StageId, "
                    + "         ActualAmount, "
                    + "         SharedAmount, "
                    + "         PaidAmount, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @StageId, "
                    + "         @ActualAmount, "
                    + "         @SharedAmount, "
                    + "         @PaidAmount, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", periodRevenueDTO.ProjectId, DbType.Guid);
                parameters.Add("StageId", periodRevenueDTO.StageId, DbType.Guid);
                parameters.Add("ActualAmount", periodRevenueDTO.ActualAmount, DbType.Double);
                parameters.Add("SharedAmount", periodRevenueDTO.SharedAmount, DbType.Double);
                parameters.Add("PaidAmount", periodRevenueDTO.PaidAmount, DbType.Double);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", periodRevenueDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", periodRevenueDTO.CreateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //SUM SHARED AMOUNT
        public async Task<double> SumSharedAmount(Guid projectId)
        {
            try
            {
                string query = "SELECT CAST(ISNULL(SUM(SharedAmount), 0) AS FLOAT) FROM PeriodRevenue WHERE ProjectId = @ProjectId AND SharedAmount IS NOT NULL";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (double)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
