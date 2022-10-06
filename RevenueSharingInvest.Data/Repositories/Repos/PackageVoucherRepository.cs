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
    public class PackageVoucherRepository : BaseRepository, IPackageVoucherRepository
    {
        public PackageVoucherRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreatePackageVoucher(PackageVoucher packageVoucherDTO)
        {
            try
            {
                var query = "INSERT INTO PackageVoucher ("
                    + "         PackageId, "
                    + "         VoucherId, "
                    + "         Quantity, "
                    + "         MaxQuantity, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     VALUES ( "
                    + "         @PackageId, "
                    + "         @VoucherId, "
                    + "         @Quantity, "
                    + "         @MaxQuantity, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageVoucherDTO.PackageId, DbType.Guid);
                parameters.Add("VoucherId", packageVoucherDTO.VoucherId, DbType.Guid);
                parameters.Add("Quantity", packageVoucherDTO.Quantity, DbType.Int16);
                parameters.Add("MaxQuantity", packageVoucherDTO.MaxQuantity, DbType.Int16);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", packageVoucherDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", packageVoucherDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePackageVoucherById(Guid packageId,Guid voucherId)
        {
            try
            {
                var query = "DELETE FROM PackageVoucher WHERE PackageId = @PackageId AND VoucherId = @VoucherId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageId, DbType.Guid);
                parameters.Add("VoucherId", voucherId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PackageVoucher>> GetAllPackageVouchers(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     PackageId ASC ) AS Num, "
                    + "             * "
                    + "         FROM PackageVoucher "
                    + "         ) "
                    + "     SELECT "
                    + "         PackageId, "
                    + "         VoucherId, "
                    + "         Quantity, "
                    + "         MaxQuantity, "
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
                    return (await connection.QueryAsync<PackageVoucher>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM PackageVoucher ORDER BY PackageId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<PackageVoucher>(query)).ToList();
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<PackageVoucher> GetPackageVoucherById(Guid packageId, Guid voucherId)
        {
            try
            {
                string query = "SELECT * FROM PackageVoucher WHERE PackageId = @PackageId AND VoucherId = @VoucherId";
                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageId, DbType.Guid);
                parameters.Add("VoucherId", voucherId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<PackageVoucher>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackageVoucher(PackageVoucher packageVoucherDTO, Guid packageId, Guid voucherId)
        {
            try
            {
                var query = "UPDATE PackageVoucher "
                    + "     SET "
                    + "         PackageId = @PackageId, "
                    + "         VoucherId = @VoucherId, "
                    + "         Quantity = @Quantity, "
                    + "         MaxQuantity = @MaxQuantity, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         PackageId=@PId "
                    + "         AND VoucherId=@VId";

                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageVoucherDTO.PackageId, DbType.Guid);
                parameters.Add("VoucherId", packageVoucherDTO.VoucherId, DbType.Guid);
                parameters.Add("Quantity", packageVoucherDTO.Quantity, DbType.Int16);
                parameters.Add("MaxQuantity", packageVoucherDTO.MaxQuantity, DbType.Int16);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", packageVoucherDTO.UpdateBy, DbType.Guid);
                parameters.Add("PId", packageId, DbType.Guid);
                parameters.Add("VId", voucherId, DbType.Guid);

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
