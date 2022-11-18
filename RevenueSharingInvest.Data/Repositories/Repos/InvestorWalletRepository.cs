using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
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
    public class InvestorWalletRepository : BaseRepository, IInvestorWalletRepository
    {
        public InvestorWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateInvestorWallet(Guid investorId, Guid walletTypeId, Guid currentUserId)
        {
            try
            {
                var query = "INSERT INTO InvestorWallet ("
                    + "         InvestorId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorId, DbType.Guid);
                parameters.Add("WalletTypeId", walletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", currentUserId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);

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
        public async Task<int> DeleteInvestorWalletById(Guid investorWalletId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                //parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorWalletId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestorWallet>> GetInvestorWalletsByInvestorId(Guid investorId)
        {
            try
            {
                var query = "SELECT " 
                    + "         IW.* "
                    + "     FROM "
                    + "         InvestorWallet IW "
                    + "         JOIN WalletType WT ON IW.WalletTypeId = WT.Id "
                    + "     WHERE "
                    + "         IW.InvestorId = @InvestorId "
                    + "     ORDER BY WT.Type ASC ";
                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestorWallet>(query, parameters)).ToList();               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY INVESTOR ID AND TYPE
        public async Task<InvestorWallet> GetInvestorWalletByInvestorIdAndType(Guid investorId, string walletType)
        {
            try
            {
                string query = "SELECT " 
                    + "             IW.Id, " 
                    + "             IW.InvestorId, " 
                    + "             IW.Balance, " 
                    + "             IW.WalletTypeId " 
                    + "         FROM " 
                    + "             InvestorWallet IW " 
                    + "             JOIN WalletType WT ON IW.WalletTypeId = WT.Id " 
                    + "         WHERE " 
                    + "             WT.Type = @Type " 
                    + "             AND IW.InvestorId = @InvestorId ";
                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorId, DbType.Guid);
                parameters.Add("Type", walletType, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorWallet>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateInvestorWallet(InvestorWallet investorWalletDTO, Guid investorWalletId)
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         InvestorId = @InvestorId, "
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorWalletId, DbType.Guid);

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
        
        //Investor Top-up 
        public async Task<int> UpdateWalletBalance(dynamic investorWalletDTO)
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         Balance = @Balance, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", investorWalletDTO.Balance, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorWalletDTO.Id, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> UpdateInvestorWalletBalance(InvestorWallet investorWalletDTO)
        {
            try
            {
                var query = "UPDATE InvestorWallet "
                    + "     SET "
                    + "         Balance = Balance + @Balance, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", investorWalletDTO.Balance, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", investorWalletDTO.Id, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<InvestorWallet> GetInvestorWalletById(Guid id)
        {
            try
            {
                string query = "SELECT * FROM InvestorWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorWallet>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<double> GetInvestorWalletBalanceById(Guid id)
        {
            try
            {
                string query = "SELECT Balance FROM InvestorWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<double>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<InvestorWallet> GetInvestorWalletByUserIdAndWalletTypeId(Guid userId, Guid walletTypeId)
        {
            try
            {
                string query = "select IW.* from InvestorWallet IW JOIN Investor I ON IW.InvestorId = I.Id where UserId = @UserId and IW.WalletTypeId = @WalletTypeId";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId, DbType.Guid);
                parameters.Add("WalletTypeId", walletTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<InvestorWallet>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        public async Task<string> GetInvertorWalletNamebyWalletId(Guid walletId)
        {
            try
            {
                string query = "SELECT WT.Name FROM InvestorWallet IW JOIN WalletType WT ON IW.WalletTypeId = WT.Id WHERE IW.Id = @WalletId";
                var parameters = new DynamicParameters();
                parameters.Add("WalletId", walletId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<string>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }
    }
}
