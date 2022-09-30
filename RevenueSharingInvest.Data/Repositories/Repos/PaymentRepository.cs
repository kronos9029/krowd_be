using Dapper;
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
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        public PaymentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreatePayment(Payment paymentDTO)
        {
            try
            {
                var query = "INSERT INTO Payment ("
                    + "         InvestmentId, "
                    + "         PeriodRevenueId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         Type, "
                    + "         FromId, "
                    + "         ToId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @InvestmentId, "
                    + "         @PeriodRevenueId, "
                    + "         @Amount, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @FromId, "
                    + "         @ToId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestmentId", paymentDTO.InvestmentId, DbType.Guid);
                parameters.Add("PeriodRevenueId", paymentDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Amount", paymentDTO.Amount, DbType.Double);
                parameters.Add("Description", paymentDTO.Description, DbType.String);
                parameters.Add("Type", paymentDTO.Type, DbType.String);
                parameters.Add("FromId", paymentDTO.FromId, DbType.Guid);
                parameters.Add("ToId", paymentDTO.ToId, DbType.Guid);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", paymentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", paymentDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeletePaymentById(Guid paymentId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE Payment "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", paymentDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", paymentId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Payment>> GetAllPayments(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     Type ASC ) AS Num, "
                    + "             * "
                    + "         FROM Payment "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvestmentId, "
                    + "         PeriodRevenueId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         Type, "
                    + "         FromId, "
                    + "         ToId, "
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
                    return (await connection.QueryAsync<Payment>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Payment WHERE IsDeleted = 0 ORDER BY Type ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Payment>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Payment> GetPaymentById(Guid paymentId)
        {
            try
            {
                string query = "SELECT * FROM Payment WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", paymentId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Payment>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePayment(Payment paymentDTO, Guid paymentId)
        {
            try
            {
                var query = "UPDATE Payment "
                    + "     SET "
                    + "         InvestmentId = @InvestmentId, "
                    + "         PeriodRevenueId = @PeriodRevenueId, "
                    + "         Amount = @Amount, "
                    + "         Description = @Description, "
                    + "         Type = @Type, "
                    + "         FromId = @FromId, "
                    + "         ToId = @ToId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestmentId", paymentDTO.InvestmentId, DbType.Guid);
                parameters.Add("PeriodRevenueId", paymentDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("Amount", paymentDTO.Amount, DbType.Double);
                parameters.Add("Description", paymentDTO.Description, DbType.String);
                parameters.Add("Type", paymentDTO.Type, DbType.String);
                parameters.Add("FromId", paymentDTO.FromId, DbType.Guid);
                parameters.Add("ToId", paymentDTO.ToId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", paymentDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", paymentDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", paymentId, DbType.Guid);

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
    }
}
