using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
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
                    + "         LastPayment, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         @ProjectId, "
                    + "         @PackageId, "
                    + "         @Quantity, "
                    + "         @TotalPrice, "
                    + "         null, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investmentDTO.InvestorId, DbType.Guid);
                parameters.Add("ProjectId", investmentDTO.ProjectId, DbType.Guid);
                parameters.Add("PackageId", investmentDTO.PackageId, DbType.Guid);
                parameters.Add("Quantity", investmentDTO.Quantity, DbType.Int16);
                parameters.Add("TotalPrice", investmentDTO.TotalPrice, DbType.Double);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investmentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteInvestmentById(Guid investmentId)
        {
            try
            {
                var query = "UPDATE Investment "
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
                parameters.Add("Id", investmentId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Investment>> GetAllInvestments(int pageIndex, int pageSize)
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
                    + "                     PackageId ASC ) AS Num, "
                    + "             * "
                    + "         FROM Investment "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         InvestorId, "
                    + "         ProjectId, "
                    + "         PackageId, "
                    + "         Quantity, "
                    + "         TotalPrice, "
                    + "         LastPayment, "
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
                    return (await connection.QueryAsync<Investment>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Investment WHERE IsDeleted = 0 ORDER BY ProjectId, PackageId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Investment>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
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
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestment(Investment investmentDTO, Guid investmentId)
        {
            try
            {
                var query = "UPDATE Investment "
                    + "     SET "
                    + "         InvestorId = @InvestorId, "
                    + "         ProjectId = @ProjectId, "
                    + "         PackageId = @PackageId, "
                    + "         Quantity = @Quantity, "
                    + "         TotalPrice = @TotalPrice, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investmentDTO.InvestorId, DbType.Guid);
                parameters.Add("ProjectId", investmentDTO.ProjectId, DbType.Guid);
                parameters.Add("PackageId", investmentDTO.PackageId, DbType.Guid);
                parameters.Add("Quantity", investmentDTO.Quantity, DbType.Int16);
                parameters.Add("TotalPrice", investmentDTO.TotalPrice, DbType.Double);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", investmentDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", investmentId, DbType.Guid);

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
        public async Task<int> ClearAllInvestmentData()
        {
            try
            {
                var query = "DELETE FROM Investment";
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
