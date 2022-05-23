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
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @PackageId, "
                    + "         @VoucherId, "
                    + "         @Quantity, "
                    + "         @MaxQuantity, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageVoucherDTO.PackageId, DbType.Guid);
                parameters.Add("VoucherId", packageVoucherDTO.VoucherId, DbType.Guid);
                parameters.Add("Quantity", packageVoucherDTO.Quantity, DbType.Int16);
                parameters.Add("MaxQuantity", packageVoucherDTO.MaxQuantity, DbType.Int16);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", packageVoucherDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", packageVoucherDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePackageVoucherById(Guid packageId,Guid voucherId)
        {
            try
            {
                var query = "UPDATE PackageVoucher "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         PackageId=@PackageId "
                    + "         AND VoucherId=@VoucherId";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("PackageId", packageId, DbType.Guid);
                parameters.Add("VoucherId", voucherId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PackageVoucher>> GetAllPackageVouchers()
        {
            try
            {
                string query = "SELECT * FROM PackageVoucher WHERE IsDeleted = 0";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<PackageVoucher>(query)).ToList();
            }
            catch (Exception e)
            {
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
                    + "         Quantity = Quantity, "
                    + "         MaxQuantity = MaxQuantity, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         PackageId=@PId "
                    + "         AND VoucherId=@VId";

                var parameters = new DynamicParameters();
                parameters.Add("PackageId", packageVoucherDTO.PackageId, DbType.Guid);
                parameters.Add("VoucherId", packageVoucherDTO.VoucherId, DbType.Guid);
                parameters.Add("Quantity", packageVoucherDTO.Quantity, DbType.Int16);
                parameters.Add("MaxQuantity", packageVoucherDTO.MaxQuantity, DbType.Int16);
                parameters.Add("CreateDate", packageVoucherDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", packageVoucherDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", packageVoucherDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", packageVoucherDTO.IsDeleted, DbType.Boolean);
                parameters.Add("PId", packageId, DbType.Guid);
                parameters.Add("VId", voucherId, DbType.Guid);

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
