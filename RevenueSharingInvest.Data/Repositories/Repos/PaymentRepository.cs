using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
                    + "         PackageId, "
                    + "         PeriodRevenueId, "
                    + "         StageId, "
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
                    + "         @PackageId, "
                    + "         @PeriodRevenueId, "
                    + "         @StageId, "
                    + "         @Amount, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @FromId, "
                    + "         @ToId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @Status )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestmentId", paymentDTO.InvestmentId, DbType.Guid);
                parameters.Add("PackageId", paymentDTO.PackageId, DbType.Guid);
                parameters.Add("PeriodRevenueId", paymentDTO.PeriodRevenueId, DbType.Guid);
                parameters.Add("StageId", paymentDTO.StageId, DbType.Guid);
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
        public async Task<List<Payment>> GetAllPayments(int pageIndex, int pageSize, string type, Guid roleId, Guid userId)
        {
            try
            {
                string whereCondition = "";
                string typeCondition = "";
                
                var parameters = new DynamicParameters();

                if (roleId.Equals(Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))))
                {
                    if (type.Equals(PaymentTypeEnum.INVESTMENT.ToString()))
                    {
                        whereCondition = whereCondition + " AND ToId = @ToId ";
                        parameters.Add("ToId", userId, DbType.Guid);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND FromId = @FromId ";
                        parameters.Add("FromId", userId, DbType.Guid);
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }
                else if (roleId.Equals(Guid.Parse(RoleDictionary.role.GetValueOrDefault("INVESTOR"))))
                {
                    if (type.Equals(PaymentTypeEnum.INVESTMENT.ToString()))
                    {
                        whereCondition = whereCondition + " AND FromId = @FromId ";
                        parameters.Add("FromId", userId, DbType.Guid);
                    }
                    else
                    {
                        whereCondition = whereCondition + " AND ToId = @ToId ";
                        parameters.Add("ToId", userId, DbType.Guid);
                    }
                    whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);
                }

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     CreateDate DESC ) AS Num, "
                    + "             * "
                    + "         FROM Payment "
                    +           whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvestmentId, "
                    + "         PackageId, "
                    + "         PeriodRevenueId, "
                    + "         StageId, "
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
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Payment>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Payment " + whereCondition + " ORDER BY CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Payment>(query, parameters)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET INVESTED AMOUNT
        public async Task<double> GetInvestedAmountForInvestorByProjectId(Guid projectId, Guid investorUserId)
        {
            try
            {
                string query = "SELECT " 
                    + "             SUM(P.Amount) AS InvestedAmount "
                    + "         FROM "
                    + "             Payment P "
                    + "             JOIN Package PK ON P.PackageId = PK.Id "
                    + "         WHERE "
                    + "             PK.ProjectId = @ProjectId "
                    + "             AND P.Type = 'INVESTMENT' "
                    + "             AND P.FromId = @FromId "
                    + "             AND P.Status = 'SUCCESS'";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("FromId", investorUserId, DbType.Guid);
                using var connection = CreateConnection();
                var result = connection.ExecuteScalar(query, parameters);
                return (result != null) ? (double)result : 0;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET LASTEST INVESTMENT DATE
        public async Task<string> GetLastestInvestmentDateForInvestorByProjectId(Guid projectId, Guid investorUserId)
        {
            try
            {
                string query = "SELECT "
                    + "             MAX(P.CreateDate) AS LastestInvestmentDate "
                    + "         FROM "
                    + "             Payment P "
                    + "             JOIN Package PK ON P.PackageId = PK.Id "
                    + "         WHERE "
                    + "             PK.ProjectId = @ProjectId "
                    + "             AND P.Type = 'INVESTMENT' "
                    + "             AND P.FromId = @FromId "
                    + "             AND P.Status = 'SUCCESS'";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("FromId", investorUserId, DbType.Guid);
                using var connection = CreateConnection();
                return connection.ExecuteScalar(query, parameters).ToString();
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

        //GET RECEIVED AMOUNT
        public async Task<double> GetReceivedAmountForInvestorByProjectId(Guid projectId, Guid investorUserId)
        {
            try
            {
                string query = "SELECT "
                    + "             SUM(P.Amount) AS ReceivedAmount "
                    + "         FROM "
                    + "             Payment P "
                    + "             JOIN Stage S ON P.StageId = S.Id "
                    + "         WHERE "
                    + "             S.ProjectId = @ProjectId "
                    + "             AND P.Type = 'PAYMENT' "
                    + "             AND P.ToId = @ToId "
                    + "             AND P.Status = 'SUCCESS'";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("ToId", investorUserId, DbType.Guid);
                using var connection = CreateConnection();
                var result = connection.ExecuteScalar(query, parameters);
                return (result != null) ? (double)result : 0;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
