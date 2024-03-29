﻿using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
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
    public class WalletTypeRepository : BaseRepository, IWalletTypeRepository
    {
        public WalletTypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateWalletType(WalletType walletTypeDTO)
        {
            try
            {
                var query = "INSERT INTO WalletType ("
                    + "         Name, "
                    + "         Description, "
                    + "         Mode, "
                    + "         Type, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @Mode, "
                    + "         @Type, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", walletTypeDTO.Name, DbType.String);
                parameters.Add("Description", walletTypeDTO.Description, DbType.String);
                parameters.Add("Mode", walletTypeDTO.Mode, DbType.String);
                parameters.Add("Type", walletTypeDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", walletTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", walletTypeDTO.UpdateBy, DbType.Guid);

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
        public async Task<List<WalletType>> GetAllWalletTypes()
        {
            try
            {
                string query = "SELECT * FROM WalletType ORDER BY Type ASC";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WalletType>(query)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<WalletType> GetWalletTypeById(Guid walletTypeId)
        {
            try
            {
                string query = "SELECT * FROM WalletType WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", walletTypeId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<WalletType>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY MODE
        public async Task<List<WalletType>> GetWalletByMode(string mode)
        {
            try
            {
                string query = "SELECT * FROM WalletType WHERE Mode = @Mode";
                var parameters = new DynamicParameters();
                parameters.Add("Mode", mode, DbType.String);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WalletType>(query)).ToList();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateWalletType(WalletType walletTypeDTO, Guid walletTypeId)
        {
            try
            {
                var query = "UPDATE WalletType "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         Description = @Description, "
                    + "         Mode = @Mode, "
                    + "         Type = @Type, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", walletTypeDTO.Name, DbType.String);
                parameters.Add("Description", walletTypeDTO.Description, DbType.String);
                parameters.Add("Mode", walletTypeDTO.Mode, DbType.String);
                parameters.Add("Type", walletTypeDTO.Type, DbType.String);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", walletTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", walletTypeId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

    }
}
