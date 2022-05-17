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
    public class ProjectWalletRepository : BaseRepository, IProjectWalletRepository
    {
        public ProjectWalletRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateProjectWallet(ProjectWallet projectWalletDTO)
        {
            try
            {
                var query = "INSERT INTO ProjectWallet ("
                    + "         ProjectId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         @Balance, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectWalletDTO.ProjectId, DbType.Guid);
                parameters.Add("Balance", projectWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
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
        public async Task<List<ProjectWallet>> GetAllProjectWallets()
        {
            try
            {
                string query = "SELECT * FROM ProjectWallet";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<ProjectWallet>(query)).ToList();
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
                    + "         ProjectId = @ProjectId, "
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
                parameters.Add("ProjectId", projectWalletDTO.ProjectId, DbType.Guid);
                parameters.Add("Balance", projectWalletDTO.Balance, DbType.Double);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", projectWalletDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", projectWalletDTO.CreateBy, DbType.Guid);
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
    }
}
