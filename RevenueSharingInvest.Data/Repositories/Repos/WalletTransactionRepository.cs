﻿using Dapper;
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
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class WalletTransactionRepository : BaseRepository, IWalletTransactionRepository
    {
        public WalletTransactionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateWalletTransaction(WalletTransaction walletTransactionDTO)
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
                    + "         CreateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
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
                    + "         @CreateBy )";

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
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", walletTransactionDTO.CreateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET ALL
        public async Task<List<WalletTransaction>> GetAllWalletTransactions(int pageIndex, int pageSize)
        {

            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     Type ASC ) AS Num, "
                    + "             * "
                    + "         FROM WalletTransaction "
                    + "         ) "
                    + "     SELECT "
                    + "         Id, "
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
                    + "         CreateBy "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WalletTransaction>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM WalletTransaction ORDER BY Type ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<WalletTransaction>(query)).ToList();
                }
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
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
                    + "         Fee = @Fee"
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
                parameters.Add("Id", walletTransactionId, DbType.Guid);

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
