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
    public class VoucherItemRepository : BaseRepository, IVoucherItemRepository
    {
        public VoucherItemRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateVoucherItem(VoucherItem voucherItemDTO)
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
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", voucherItemDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", voucherItemDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
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
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", voucherItemDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", voucherItemId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<VoucherItem>> GetAllVoucherItems()
        {
            try
            {
                string query = "SELECT * FROM VoucherItem";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<VoucherItem>(query)).ToList();
            }
            catch (Exception e)
            {
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
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
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
                parameters.Add("CreateDate", voucherItemDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", voucherItemDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
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
                throw new Exception(e.Message, e);
            }
        }
    }
}
