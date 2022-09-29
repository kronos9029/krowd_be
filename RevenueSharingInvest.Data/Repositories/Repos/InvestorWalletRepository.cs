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
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @InvestorId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

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
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
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
                var query = " SELECT IW.* FROM InvestorWallet IW JOIN WalletType WT ON IW.WalletTypeId = WT.Id WHERE IW.IsDeleted = 0 "
                    + " AND IW.InvestorId = @InvestorId "
                    + " AND (IW.WalletTypeId = @I1 "
                    + "     OR IW.WalletTypeId = @I2 "
                    + "     OR IW.WalletTypeId = @I3 "
                    + "     OR IW.WalletTypeId = @I4 "
                    + "     OR IW.WalletTypeId = @I5 ) "
                    + " ORDER BY WT.Type ASC ";
                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorId, DbType.Guid);
                parameters.Add("I1", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I1")), DbType.Guid);
                parameters.Add("I2", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")), DbType.Guid);
                parameters.Add("I3", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3")), DbType.Guid);
                parameters.Add("I4", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4")), DbType.Guid);
                parameters.Add("I5", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5")), DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<InvestorWallet>(query, parameters)).ToList();               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<InvestorWallet> GetInvestorWalletByTypeAndInvestorId(Guid investorId, string walletType)
        {
            try
            {
                string query = "SELECT IW.Id, IW.InvestorId, IW.Balance, IW.WalletTypeId " +
                    "FROM InvestorWallet IW JOIN WalletType WT ON IW.WalletTypeId = WT.Id " +
                    "WHERE WT.Type = @Type AND IW.InvestorId = @InvestorId AND IW.IsDeleted = 0";
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
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", investorWalletDTO.IsDeleted, DbType.Boolean);
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
        public async Task<int> UpdateWalletBalance(InvestorWallet investorWalletDTO)
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
        public async Task<int> ClearAllInvestorWalletData()
        {
            try
            {
                var query = "DELETE FROM InvestorWallet";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
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
                    + "         InvestorId = @InvestorId" 
                    + "         AND WalletTypeId = @WalletTypeId";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", investorWalletDTO.Balance, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", investorWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("InvestorId", investorWalletDTO.InvestorId, DbType.Guid);
                parameters.Add("WalletTypeId", investorWalletDTO.WalletTypeId, DbType.Guid);

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
