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
    public class InvestorTypeRepository : BaseRepository, IInvestorTypeRepository
    {
        public InvestorTypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateInvestorType(InvestorType investorTypeDTO)
        {
            try
            {
                var query = "INSERT INTO InvestorType ("
                    + "         Name, "
                    + "         Description, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", investorTypeDTO.Name, DbType.String);
                parameters.Add("Description", investorTypeDTO.Description, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", investorTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorTypeDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteInvestorTypeById(Guid investorTypeId)
        {
            try
            {
                var query = "DELETE FROM InvestorType WHERE Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", investorTypeId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestorType>> GetAllInvestorTypes(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM InvestorType "
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         Description, "
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
                    return (await connection.QueryAsync<InvestorType>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM InvestorType ORDER BY Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<InvestorType>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<InvestorType> GetInvestorTypeById(Guid investorTypeId)
        {
            try
            {
                string query = "SELECT * FROM InvestorType WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investorTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorType>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorType(InvestorType investorTypeDTO, Guid investorTypeId)
        {
            try
            {
                var query = "UPDATE InvestorType "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Description = @Description, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", investorTypeDTO.Name, DbType.String);
                parameters.Add("Description", investorTypeDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorTypeId, DbType.Guid);

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

        //GET BY INVESTOR ID
        public async Task<InvestorType> GetInvestorTypeByInvestorId(Guid investorId)
        {
            try
            {
                string query = "SELECT IT.* FROM InvestorType IT JOIN Investor I ON IT.Id = I.InvestorTypeId WHERE I.Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorType>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
