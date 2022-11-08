using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.DTOs;
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
    public class InvestmentRepository : BaseRepository, IInvestmentRepository
    {
        public InvestmentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateInvestment(Investment investmentDTO)
        {
            try
            {
                var query = "INSERT INTO Investment ("
                    + "         InvestorId, "
                    + "         ProjectId, "
                    + "         PackageId, "
                    + "         Quantity, "
                    + "         TotalPrice, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         @ProjectId, "
                    + "         @PackageId, "
                    + "         @Quantity, "
                    + "         @TotalPrice, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investmentDTO.InvestorId, DbType.Guid);
                parameters.Add("ProjectId", investmentDTO.ProjectId, DbType.Guid);
                parameters.Add("PackageId", investmentDTO.PackageId, DbType.Guid);
                parameters.Add("Quantity", investmentDTO.Quantity, DbType.Int16);
                parameters.Add("TotalPrice", investmentDTO.TotalPrice, DbType.Double);
                parameters.Add("Status", investmentDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", investmentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);

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
        public async Task<List<Investment>> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, Guid roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var projectStatusCondition = " AND P.Status = @Status ";
                var businessIdCondition = " AND P.BusinessId = @BusinessId ";
                var projectIdCondition = " AND I.ProjectId = @ProjectId ";
                var investorIdCondition = " AND I.InvestorId = @InvestorId ";

                if (walletTypeId != null)
                {
                    if (Guid.Parse(walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"))))
                    {
                        whereCondition = whereCondition + " AND (P.Status = @CALLING_FOR_INVESTMENT OR P.Status = @CALLING_TIME_IS_OVER OR P.Status = @WAITING_TO_ACTIVATE) ";
                        parameters.Add("CALLING_FOR_INVESTMENT", ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), DbType.String);
                        parameters.Add("CALLING_TIME_IS_OVER", ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString(), DbType.String);
                        parameters.Add("WAITING_TO_ACTIVATE", ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString(), DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + projectStatusCondition;
                        parameters.Add("Status", ProjectStatusEnum.ACTIVE.ToString(), DbType.String);
                    }              
                }
                else
                {
                    whereCondition = whereCondition + " AND (P.Status = @CALLING_FOR_INVESTMENT OR P.Status = @CALLING_TIME_IS_OVER OR P.Status = @WAITING_TO_ACTIVATE OR P.Status = @ACTIVE) ";
                    parameters.Add("CALLING_FOR_INVESTMENT", ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), DbType.String);
                    parameters.Add("CALLING_TIME_IS_OVER", ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString(), DbType.String);
                    parameters.Add("WAITING_TO_ACTIVATE", ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString(), DbType.String);
                    parameters.Add("ACTIVE", ProjectStatusEnum.ACTIVE.ToString(), DbType.String);
                }
                if (businessId != null)
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                }
                if (projectId != null)
                {
                    whereCondition = whereCondition + projectIdCondition;
                    parameters.Add("ProjectId", Guid.Parse(projectId), DbType.Guid);
                }
                if (investorId != null)
                {
                    whereCondition = whereCondition + investorIdCondition;
                    parameters.Add("InvestorId", Guid.Parse(investorId), DbType.Guid);
                }
                whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);

                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     I.CreateDate DESC ) AS Num, "
                    + "             I.* "
                    + "         FROM " 
                    + "             Investment I " 
                    + "                 JOIN Project P ON I.ProjectId = P.Id "
                    +           whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvestorId, "
                    + "         ProjectId, "
                    + "         PackageId, "
                    + "         Quantity, "
                    + "         TotalPrice, "
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
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Investment>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT "
                    + "         I.* "
                    + "     FROM "
                    + "         Investment I JOIN Project P ON I.ProjectId = P.Id "
                    +       whereCondition
                    + "     ORDER BY "
                    + "         I.CreateDate DESC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Investment>(query, parameters)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Investment> GetInvestmentById(Guid investmentId)
        {
            try
            {
                string query = "SELECT * FROM Investment WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investmentId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Investment>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }        
        //GET INVESTMENT FOR AUTHOR
        public async Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId)
        {
            try
            {
                string query = "SELECT DISTINCT u.Id AS InvestorId, u.UserId, i.ProjectId " +
                    "FROM Investor u " +
                    "JOIN Investment i ON u.Id = i.InvestorId " +
                    "WHERE i.ProjectId = @ProjectId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestorInvestmentDTO>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        //public async Task<int> UpdateInvestment(Investment investmentDTO, Guid investmentId)
        //{
        //    try
        //    {
        //        var query = "UPDATE Investment "
        //            + "     SET "
        //            + "         InvestorId = @InvestorId, "
        //            + "         ProjectId = @ProjectId, "
        //            + "         PackageId = @PackageId, "
        //            + "         Quantity = @Quantity, "
        //            + "         TotalPrice = @TotalPrice, "
        //            + "         UpdateDate = @UpdateDate, "
        //            + "         UpdateBy = @UpdateBy"
        //            + "     WHERE "
        //            + "         Id = @Id";

        //        var parameters = new DynamicParameters();
        //        parameters.Add("InvestorId", investmentDTO.InvestorId, DbType.Guid);
        //        parameters.Add("ProjectId", investmentDTO.ProjectId, DbType.Guid);
        //        parameters.Add("PackageId", investmentDTO.PackageId, DbType.Guid);
        //        parameters.Add("Quantity", investmentDTO.Quantity, DbType.Int16);
        //        parameters.Add("TotalPrice", investmentDTO.TotalPrice, DbType.Double);
        //        parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
        //        parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
        //        parameters.Add("Id", investmentId, DbType.Guid);

        //        using (var connection = CreateConnection())
        //        {
        //            return await connection.ExecuteAsync(query, parameters);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}

        //GET FOR WALLET
        //public async Task<List<Investment>> GetInvestmentForWallet(Guid investorId, string status)
        //{
        //    try
        //    {
        //        var query = "SELECT " 
        //            + "         I.* " 
        //            + "     FROM " 
        //            + "         Investment I JOIN Project P ON I.ProjectId = P.Id "
        //            + "     WHERE " 
        //            + "         I.InvestorId = @InvestorId AND P.Status = @Status "
        //            + "     ORDER BY " 
        //            + "         I.CreateDate DESC";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("InvestorId", investorId, DbType.Guid);
        //        parameters.Add("Status", status, DbType.String);
        //        using var connection = CreateConnection();
        //        return (await connection.QueryAsync<Investment>(query, parameters)).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}

        //UPDATE STATUS
        public async Task<int> UpdateInvestmentStatus(Investment investmentDTO)
        {
            try
            {
                var query = "UPDATE Investment "
                    + "     SET "
                    + "         Status = @Status, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Status", investmentDTO.Status, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investmentDTO.Id, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<List<InvestedRecord>> GetInvestmentRecord(Guid projectId, Guid investorId)
        {
            try
            {
                var query = "SELECT PA.Name AS PackageName, IV.Quantity, IV.TotalPrice, IV.CreateDate FROM Investment IV JOIN Package PA ON IV.PackageId = PA.Id WHERE IV.ProjectId = @ProjectId AND IV.InvestorId = @InvestorId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("InvestorId", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestedRecord>(query, parameters)).ToList();

            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //COUNT BY PROJECT ID AND INVESTOR ID
        public async Task<int> CountInvestmentByProjectAndInvestor(Guid projectId, Guid investorId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Investment WHERE ProjectId = @ProjectId AND InvestorId = @InvestorId";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                parameters.Add("InvestorId", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }


        //COUNT ALL
        public async Task<int> CountAllInvestments(string walletTypeId, string businessId, string projectId, string investorId, Guid roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var projectStatusCondition = " AND P.Status = @Status ";
                var businessIdCondition = " AND P.BusinessId = @BusinessId ";
                var projectIdCondition = " AND I.ProjectId = @ProjectId ";
                var investorIdCondition = " AND I.InvestorId = @InvestorId ";

                if (walletTypeId != null)
                {
                    if (Guid.Parse(walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"))))
                    {
                        whereCondition = whereCondition + " AND (P.Status = @CALLING_FOR_INVESTMENT OR P.Status = @CALLING_TIME_IS_OVER OR P.Status = @WAITING_TO_ACTIVATE) ";
                        parameters.Add("CALLING_FOR_INVESTMENT", ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), DbType.String);
                        parameters.Add("CALLING_TIME_IS_OVER", ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString(), DbType.String);
                        parameters.Add("WAITING_TO_ACTIVATE", ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString(), DbType.String);
                    }
                    else
                    {
                        whereCondition = whereCondition + projectStatusCondition;
                        parameters.Add("Status", ProjectStatusEnum.ACTIVE.ToString(), DbType.String);
                    }
                }
                else
                {
                    whereCondition = whereCondition + " AND (P.Status = @CALLING_FOR_INVESTMENT OR P.Status = @CALLING_TIME_IS_OVER OR P.Status = @WAITING_TO_ACTIVATE OR P.Status = @ACTIVE) ";
                    parameters.Add("CALLING_FOR_INVESTMENT", ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), DbType.String);
                    parameters.Add("CALLING_TIME_IS_OVER", ProjectStatusEnum.CALLING_TIME_IS_OVER.ToString(), DbType.String);
                    parameters.Add("WAITING_TO_ACTIVATE", ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString(), DbType.String);
                    parameters.Add("ACTIVE", ProjectStatusEnum.ACTIVE.ToString(), DbType.String);
                }
                if (businessId != null)
                {
                    whereCondition = whereCondition + businessIdCondition;
                    parameters.Add("BusinessId", Guid.Parse(businessId), DbType.Guid);
                }
                if (projectId != null)
                {
                    whereCondition = whereCondition + projectIdCondition;
                    parameters.Add("ProjectId", Guid.Parse(projectId), DbType.Guid);
                }
                if (investorId != null)
                {
                    whereCondition = whereCondition + investorIdCondition;
                    parameters.Add("InvestorId", Guid.Parse(investorId), DbType.Guid);
                }
                whereCondition = "WHERE " + whereCondition.Substring(4, whereCondition.Length - 4);

                var query = " SELECT COUNT(*) FROM (SELECT "
                + "                                     I.* "
                + "                                 FROM "
                + "                                     Investment I JOIN Project P ON I.ProjectId = P.Id "
                +                                   whereCondition
                + "                                 ) AS X";
                using var connection = CreateConnection();
                return (int)connection.ExecuteScalar(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
