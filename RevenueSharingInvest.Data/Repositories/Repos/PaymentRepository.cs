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
                    + "         Status ) "
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
                    + "         @CreateBy "
                    + "         @Status )";

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
                parameters.Add("Status", paymentDTO.Status, DbType.String);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
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
                    + "         ) "
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
                    + "         Status "
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
                    var query = "SELECT * FROM Payment ORDER BY Type ASC";
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
    }
}
