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
    public class ProjectWalletRepository : BaseRepository, IProjectWalletRepository
    {
        public ProjectWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<string> CreateProjectWallet(ProjectWallet projectWalletDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectWallet ("
                    + "         ProjectManagerId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         ProjectId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectManagerId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @ProjectId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectWalletDTO.ProjectManagerId, DbType.Guid);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("ProjectId", projectWalletDTO.ProjectId, DbType.Guid);
                parameters.Add("CreateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("CreateBy", projectWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.CreateBy, DbType.Guid);

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
        public async Task<List<ProjectWallet>> GetProjectWalletsByProjectManagerId(Guid projectManagerId)
        {
            try
            {
                var query = "SELECT " 
                    + "         PW.* " 
                    + "     FROM " 
                    + "         ProjectWallet PW " 
                    + "         JOIN WalletType WT ON PW.WalletTypeId = WT.Id "
                    + "     WHERE " 
                    + "         PW.ProjectManagerId = @ProjectManagerId "
                    + "     ORDER BY WT.Type ASC ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectManagerId, DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectWallet>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<ProjectWallet> GetProjectWalletById(Guid id)
        {
            try
            {
                string query = "SELECT * FROM ProjectWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ProjectWallet>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateProjectWallet(ProjectWallet projectWalletDTO, Guid projectWalletId)
        {
            try
            {
                var query = "UPDATE ProjectWallet "
                    + "     SET "
                    + "         ProjectManagerId = @ProjectManagerId, "
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectWalletDTO.ProjectManagerId, DbType.Guid);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectWalletId, DbType.Guid);

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

        //DELETE BY BUSINESS ID
        public async Task<int> DeleteProjectWalletByBusinessId(Guid businessId)
        {
            try
            {
                var query = "DELETE FROM ProjectWallet "
                    + "     WHERE "
                    + "         ProjectManagerId IN (SELECT Id FROM [User] WHERE BusinessId = @BusinessId) ";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", businessId, DbType.Guid);
                //parameters.Add("B2", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P3")), DbType.Guid);
                //parameters.Add("B3", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4")), DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY PROJECT OWNER ID AND TYPE
        public async Task<ProjectWallet> GetProjectWalletByProjectOwnerIdAndType(Guid projectOwnerId, string walletType)
        {
            try
            {
                string query = "SELECT "
                    + "             PW.Id, "
                    + "             PW.ProjectManagerId, "
                    + "             PW.Balance, "
                    + "             PW.WalletTypeId "
                    + "         FROM "
                    + "             ProjectWallet PW "
                    + "             JOIN WalletType WT ON PW.WalletTypeId = WT.Id "
                    + "         WHERE "
                    + "             WT.Type = @Type "
                    + "             AND PW.ProjectManagerId = @ProjectManagerId ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectOwnerId, DbType.Guid);
                parameters.Add("Type", walletType, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ProjectWallet>(query, parameters);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE BALANCE
        public async Task<int> UpdateProjectWalletBalance(ProjectWallet projectWalletDTO)
        {
            try
            {
                var query = "UPDATE ProjectWallet "
                    + "     SET "
                    + "         Balance = Balance + @Balance, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy"
                    + "     WHERE "
                    + "         ProjectManagerId = @ProjectManagerId"
                    + "         AND WalletTypeId = @WalletTypeId";

                var parameters = new DynamicParameters();
                parameters.Add("Balance", projectWalletDTO.Balance, DbType.Double);
                parameters.Add("UpdateDate", DateTimePicker.GetDateTimeByTimeZone(), DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("ProjectManagerId", projectWalletDTO.ProjectManagerId, DbType.Guid);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);

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
