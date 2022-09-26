using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
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
        public async Task<string> CreateProjectWallet(Guid projectManagerId, Guid walletTypeId, Guid currentUserId)
        {
            try
            {
                var query = "INSERT INTO ProjectWallet ("
                    + "         ProjectManagerId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectManagerId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectManagerId, DbType.Guid);
                parameters.Add("WalletTypeId", walletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", currentUserId, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", currentUserId, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectWalletById(Guid projectWalletId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE ProjectWallet "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", projectWalletId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<ProjectWallet>> GetProjectWalletsByProjectManagerId(Guid projectManagerId)
        {
            try
            {
                var query = " SELECT PW.* FROM ProjectWallet PW JOIN WalletType WT ON PW.WalletTypeId = WT.Id WHERE PW.IsDeleted = 0 "
                    + " AND PW.ProjectManagerId = @ProjectManagerId "
                    + " AND ( PW.WalletTypeId = @B1 " 
                    + "     OR PW.WalletTypeId = @B2 "
                    + "     OR PW.WalletTypeId = @B3 "
                    + "     OR PW.WalletTypeId = @B4 ) "
                    + " ORDER BY WT.Type ASC ";
                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectManagerId, DbType.Guid);
                parameters.Add("B1", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B1")), DbType.Guid);
                parameters.Add("B2", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B2")), DbType.Guid);
                parameters.Add("B3", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")), DbType.Guid);
                parameters.Add("B4", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B4")), DbType.Guid);
                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectWallet>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<ProjectWallet> GetProjectWalletById(Guid projectWalletId)
        {
            try
            {
                string query = "SELECT * FROM ProjectWallet WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", projectWalletId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ProjectWallet>(query, parameters);
            }
            catch (Exception e)
            {
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
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectManagerId", projectWalletDTO.ProjectManagerId, DbType.Guid);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", projectWalletDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", projectWalletId, DbType.Guid);

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

        //CLEAR DATA
        public async Task<int> ClearAllProjectWalletData()
        {
            try
            {
                var query = "DELETE FROM ProjectWallet";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                //parameters.Add("B2", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B2")), DbType.Guid);
                //parameters.Add("B3", Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("B3")), DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
