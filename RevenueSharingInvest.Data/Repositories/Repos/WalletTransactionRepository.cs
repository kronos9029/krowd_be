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
    public class WalletTransactionRepository : BaseRepository, IWalletTransactionRepository
    {
        public WalletTransactionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateWalletTransaction(WalletTransaction walletTransactionDTO)
        {
            try
            {
                var query = "INSERT INTO WalletTransaction ("
                    + "         PaymentId, "
                    + "         SystemWalletId, "
                    + "         ProjectWalletId, "
                    + "         InvestorWalletId, "
                    + "         Amount, "
                    + "         Description, "
                    + "         Type, "
                    + "         FromWalletId, "
                    + "         ToWalletId, "
                    + "         Fee, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @PaymentId, "
                    + "         @SystemWalletId, "
                    + "         @ProjectWalletId, "
                    + "         @InvestorWalletId, "
                    + "         @Amount, "
                    + "         @Description, "
                    + "         @Type, "
                    + "         @FromWalletId, "
                    + "         @ToWalletId, "
                    + "         @Fee, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("PaymentId", walletTransactionDTO.PaymentId, DbType.Guid);
                parameters.Add("SystemWalletId", walletTransactionDTO.SystemWalletId, DbType.Guid);
                parameters.Add("ProjectWalletId", walletTransactionDTO.ProjectWalletId, DbType.Guid);
                parameters.Add("InvestorWalletId", walletTransactionDTO.InvestorWalletId, DbType.Guid);
                parameters.Add("Amount", walletTransactionDTO.Amount, DbType.Double);
                parameters.Add("Description", walletTransactionDTO.Description, DbType.String);
                parameters.Add("Type", walletTransactionDTO.Type, DbType.String);
                parameters.Add("FromWalletId", walletTransactionDTO.FromWalletId, DbType.Guid);
                parameters.Add("ToWalletId", walletTransactionDTO.ToWalletId, DbType.Guid);
                parameters.Add("Fee", walletTransactionDTO.Fee, DbType.Double);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", walletTransactionDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", walletTransactionDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteWalletTransactionById(Guid walletTransactionId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE WalletTransaction "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", walletTransactionDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", walletTransactionId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<WalletTransaction>> GetAllWalletTransactions()
        {
            try
            {
                string query = "SELECT * FROM WalletTransaction";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WalletTransaction>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<WalletTransaction> GetWalletTransactionById(Guid walletTransactionId)
        {
            try
            {
                string query = "SELECT * FROM WalletTransaction WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", walletTransactionId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<WalletTransaction>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletTransaction(WalletTransaction walletTransactionDTO, Guid walletTransactionId)
        {
            try
            {
                var query = "UPDATE WalletTransaction "
                    + "     SET "
                    + "         PaymentId = @PaymentId, "
                    + "         SystemWalletId = @SystemWalletId, "
                    + "         ProjectWalletId = @ProjectWalletId, "
                    + "         InvestorWalletId = @InvestorWalletId, "
                    + "         Amount = @Amount, "
                    + "         Description = @Description, "
                    + "         Type = @Type, "
                    + "         FromWalletId = @FromWalletId, "
                    + "         ToWalletId = @ToWalletId, "
                    + "         Fee = @Fee, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("PaymentId", walletTransactionDTO.PaymentId, DbType.Guid);
                parameters.Add("SystemWalletId", walletTransactionDTO.SystemWalletId, DbType.Guid);
                parameters.Add("ProjectWalletId", walletTransactionDTO.ProjectWalletId, DbType.Guid);
                parameters.Add("InvestorWalletId", walletTransactionDTO.InvestorWalletId, DbType.Guid);
                parameters.Add("Amount", walletTransactionDTO.Amount, DbType.Double);
                parameters.Add("Description", walletTransactionDTO.Description, DbType.String);
                parameters.Add("Type", walletTransactionDTO.Type, DbType.String);
                parameters.Add("FromWalletId", walletTransactionDTO.FromWalletId, DbType.Guid);
                parameters.Add("ToWalletId", walletTransactionDTO.ToWalletId, DbType.Guid);
                parameters.Add("Fee", walletTransactionDTO.Fee, DbType.Double);
                parameters.Add("CreateDate", walletTransactionDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", walletTransactionDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", walletTransactionDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", walletTransactionDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", walletTransactionId, DbType.Guid);

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
