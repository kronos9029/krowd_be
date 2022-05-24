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
    public class InvestmentRepository : BaseRepository, IInvestmentRepository
    {
        public InvestmentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateInvestment(Investment investmentDTO)
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
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         @ProjectId, "
                    + "         @PackageId, "
                    + "         @Quantity, "
                    + "         @TotalPrice, "
                    + "         @LastPayment, "
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
                parameters.Add("LastPayment", investmentDTO.LastPayment, DbType.DateTime);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", investmentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
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
        public async Task<List<Investment>> GetAllInvestments()
        {
            try
            {
                string query = "SELECT * FROM Investment WHERE IsDeleted = 0";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Investment>(query)).ToList();
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
                    + "         LastPayment = @LastPayment, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
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
                parameters.Add("LastPayment", investmentDTO.LastPayment, DbType.DateTime);
                parameters.Add("CreateDate", investmentDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", investmentDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", investmentDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", investmentDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", investmentDTO, DbType.Guid);

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
