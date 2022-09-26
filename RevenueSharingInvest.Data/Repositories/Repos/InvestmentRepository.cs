using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
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
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investmentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
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

        //DELETE
        //public async Task<int> DeleteInvestmentById(Guid investmentId)
        //{
        //    try
        //    {
        //        var query = "UPDATE Investment "
        //            + "     SET "
        //            + "         UpdateDate = @UpdateDate, "
        //            //+ "         UpdateBy = @UpdateBy, "
        //            + "         IsDeleted = 1 "
        //            + "     WHERE "
        //            + "         Id=@Id";
        //        using var connection = CreateConnection();
        //        var parameters = new DynamicParameters();
        //        parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
        //        //parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
        //        parameters.Add("Id", investmentId, DbType.Guid);

        //        return await connection.ExecuteAsync(query, parameters);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //GET ALL
        public async Task<List<Investment>> GetAllInvestments(int pageIndex, int pageSize, string projectStatus, string businessId, string projectId, string investorId, Guid roleId)
        {
            try
            {
                var parameters = new DynamicParameters();

                var whereCondition = "";

                var projectStatusCondition = " AND P.Status = @Status ";
                var businessIdCondition = " AND P.BusinessId = @BusinessId ";
                var projectIdCondition = " AND I.ProjectId = @ProjectId ";
                var investorIdCondition = " AND I.InvestorId = @InvestorId ";

                if (projectStatus != null)
                {
                    whereCondition = whereCondition + projectStatusCondition;
                    parameters.Add("Status", projectStatus, DbType.String);
                }
                else
                {
                    whereCondition = whereCondition + " AND P.Status = @I3 AND P.Status = @I4 ";
                    parameters.Add("I3", ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString(), DbType.String);
                    parameters.Add("I4", ProjectStatusEnum.ACTIVE.ToString(), DbType.String);
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
                    + "             * "
                    + "         FROM " 
                    + "             Investment I " 
                    + "                 JOIN Project P ON I.ProjectId = P.Id "
                    +           whereCondition
                    + "         ) "
                    + "     SELECT "
                    + "         I.Id, "
                    + "         I.InvestorId, "
                    + "         I.ProjectId, "
                    + "         I.PackageId, "
                    + "         I.Quantity, "
                    + "         I.TotalPrice, "
                    + "         I.Status, "
                    + "         I.CreateDate, "
                    + "         I.CreateBy, "
                    + "         I.UpdateDate, "
                    + "         I.UpdateBy "
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
        //            + "         UpdateBy = @UpdateBy, "
        //            + "         IsDeleted = @IsDeleted"
        //            + "     WHERE "
        //            + "         Id = @Id";

        //        var parameters = new DynamicParameters();
        //        parameters.Add("InvestorId", investmentDTO.InvestorId, DbType.Guid);
        //        parameters.Add("ProjectId", investmentDTO.ProjectId, DbType.Guid);
        //        parameters.Add("PackageId", investmentDTO.PackageId, DbType.Guid);
        //        parameters.Add("Quantity", investmentDTO.Quantity, DbType.Int16);
        //        parameters.Add("TotalPrice", investmentDTO.TotalPrice, DbType.Double);
        //        parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
        //        parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
        //        parameters.Add("IsDeleted", investmentDTO.IsDeleted, DbType.Boolean);
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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investmentDTO.Id, DbType.Guid);

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
