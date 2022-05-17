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
    public class SystemWalletRepository : BaseRepository, ISystemWalletRepository
    {
        public SystemWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateSystemWallet(SystemWallet systemWalletDTO)
        {
            try
            {
                var query = "INSERT INTO SystemWallet ("
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @Balance, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", systemWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", systemWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", systemWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", systemWalletDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteSystemWalletById(Guid systemWalletId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE SystemWallet "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", systemWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", systemWalletId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<SystemWallet>> GetAllSystemWallets()
        {
            try
            {
                string query = "SELECT * FROM SystemWallet";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<SystemWallet>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<SystemWallet> GetSystemWalletById(Guid systemWalletId)
        {
            try
            {
                string query = "SELECT * FROM SystemWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", systemWalletId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<SystemWallet>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateSystemWallet(SystemWallet systemWalletDTO, Guid systemWalletId)
        {
            try
            {
                var query = "UPDATE SystemWallet "
                    + "     SET "
                    + "         Balance = @Balance, "
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", systemWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", systemWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", systemWalletDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", systemWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", systemWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", systemWalletDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", systemWalletId, DbType.Guid);

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
