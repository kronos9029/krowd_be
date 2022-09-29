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
    public class VoucherItemRepository : BaseRepository, IVoucherItemRepository
    {
        public VoucherItemRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateVoucherItem(VoucherItem voucherItemDTO)
        {
            try
            {
                var query = "INSERT INTO VoucherItem ("
                    + "         VoucherId, "
                    + "         InvestmentId, "
                    + "         IssuedDate, "
                    + "         ExpireDate, "
                    + "         RedeemDate, "
                    + "         AvailableDate, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @VoucherId, "
                    + "         @InvestmentId, "
                    + "         @IssuedDate, "
                    + "         @ExpireDate, "
                    + "         @RedeemDate, "
                    + "         @AvailableDate, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("VoucherId", voucherItemDTO.VoucherId, DbType.Guid);
                parameters.Add("InvestmentId", voucherItemDTO.InvestmentId, DbType.Guid);
                parameters.Add("IssuedDate", voucherItemDTO.IssuedDate, DbType.DateTime);
                parameters.Add("ExpireDate", voucherItemDTO.ExpireDate, DbType.DateTime);
                parameters.Add("RedeemDate", voucherItemDTO.RedeemDate, DbType.DateTime);
                parameters.Add("AvailableDate", voucherItemDTO.AvailableDate, DbType.DateTime);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", voucherItemDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", voucherItemDTO.UpdateBy, DbType.Guid);

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
        public async Task<int> DeleteVoucherItemById(Guid voucherItemId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE VoucherItem "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                //parameters.Add("UpdateBy", voucherItemDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", voucherItemId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<VoucherItem>> GetAllVoucherItems(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     VoucherId ASC ) AS Num, "
                    + "             * "
                    + "         FROM VoucherItem "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         VoucherId, "
                    + "         InvestmentId, "
                    + "         IssuedDate, "
                    + "         ExpireDate, "
                    + "         RedeemDate, "
                    + "         AvailableDate, "
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
                    return (await connection.QueryAsync<VoucherItem>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM VoucherItem WHERE IsDeleted = 0 ORDER BY VoucherId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<VoucherItem>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<VoucherItem> GetVoucherItemById(Guid voucherItemId)
        {
            try
            {
                string query = "SELECT * FROM VoucherItem WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", voucherItemId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<VoucherItem>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateVoucherItem(VoucherItem voucherItemDTO, Guid voucherItemId)
        {
            try
            {
                var query = "UPDATE VoucherItem "
                    + "     SET "
                    + "         VoucherId = @VoucherId, "
                    + "         InvestmentId = @InvestmentId, "
                    + "         IssuedDate = @IssuedDate, "
                    + "         ExpireDate = @ExpireDate, "
                    + "         RedeemDate = @RedeemDate, "
                    + "         AvailableDate = @AvailableDate, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("VoucherId", voucherItemDTO.VoucherId, DbType.Guid);
                parameters.Add("InvestmentId", voucherItemDTO.InvestmentId, DbType.Guid);
                parameters.Add("IssuedDate", voucherItemDTO.IssuedDate, DbType.DateTime);
                parameters.Add("ExpireDate", voucherItemDTO.ExpireDate, DbType.DateTime);
                parameters.Add("RedeemDate", voucherItemDTO.RedeemDate, DbType.DateTime);
                parameters.Add("AvailableDate", voucherItemDTO.AvailableDate, DbType.DateTime);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", voucherItemDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", voucherItemDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", voucherItemId, DbType.Guid);

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

        //CLEAR DATA
        public async Task<int> ClearAllVoucherItemData()
        {
            try
            {
                var query = "DELETE FROM VoucherItem";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}
