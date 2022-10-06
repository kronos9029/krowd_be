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
    public class VoucherRepository : BaseRepository, IVoucherRepository
    {
        public VoucherRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateVoucher(Voucher voucherDTO)
        {
            try
            {
                var query = "INSERT INTO Voucher ("
                    + "         Name, "
                    + "         Code, "
                    + "         ProjectId, "
                    + "         Description, "
                    + "         Image, "
                    + "         Quantity, "
                    + "         Status, "
                    + "         StartDate, "
                    + "         EndDate, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Code, "
                    + "         @ProjectId, "
                    + "         @Description, "
                    + "         @Image, "
                    + "         @Quantity, "
                    + "         @Status, "
                    + "         @StartDate, "
                    + "         @EndDate, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", voucherDTO.Name, DbType.String);
                parameters.Add("Code", voucherDTO.Code, DbType.String);
                parameters.Add("ProjectId", voucherDTO.ProjectId, DbType.Guid);
                parameters.Add("Description", voucherDTO.Description, DbType.String);
                parameters.Add("Image", voucherDTO.Image, DbType.String);
                parameters.Add("Quantity", voucherDTO.Quantity, DbType.Int16);
                parameters.Add("Status", voucherDTO.Status, DbType.String);
                parameters.Add("StartDate", Convert.ToDateTime(voucherDTO.StartDate), DbType.DateTime);
                parameters.Add("EndDate", Convert.ToDateTime(voucherDTO.StartDate), DbType.DateTime);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", voucherDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", voucherDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteVoucherById(Guid voucherId)
        {
            try
            {
                var query = "DELETE FROM Voucher WHERE Id = @Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", voucherId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Voucher>> GetAllVouchers(int pageIndex, int pageSize)
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
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Voucher "
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         Code, "
                    + "         ProjectId, "
                    + "         Description, "
                    + "         Image, "
                    + "         Quantity, "
                    + "         Status, "
                    + "         StartDate, "
                    + "         EndDate, "
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
                    return (await connection.QueryAsync<Voucher>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Voucher ORDER BY ProjectId, Name ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Voucher>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Voucher> GetVoucherById(Guid voucherId)
        {
            try
            {
                string query = "SELECT * FROM Voucher WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", voucherId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Voucher>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateVoucher(Voucher voucherDTO, Guid voucherId)
        {
            try
            {
                var query = "UPDATE Voucher "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Code = @Code, "
                    + "         ProjectId = @ProjectId, "
                    + "         Description = @Description, "
                    + "         Image = @Image, "
                    + "         Quantity = @Quantity, "
                    + "         Status = @Status, "
                    + "         StartDate = @StartDate, "
                    + "         EndDate = @EndDate, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", voucherDTO.Name, DbType.String);
                parameters.Add("Code", voucherDTO.Code, DbType.String);
                parameters.Add("ProjectId", voucherDTO.ProjectId, DbType.Guid);
                parameters.Add("Description", voucherDTO.Description, DbType.String);
                parameters.Add("Image", voucherDTO.Image, DbType.String);
                parameters.Add("Quantity", voucherDTO.Quantity, DbType.Int16);
                parameters.Add("Status", voucherDTO.Status, DbType.String);
                parameters.Add("StartDate", voucherDTO.StartDate, DbType.DateTime);
                parameters.Add("EndDate", voucherDTO.EndDate, DbType.DateTime);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", voucherDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", voucherId, DbType.Guid);

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
