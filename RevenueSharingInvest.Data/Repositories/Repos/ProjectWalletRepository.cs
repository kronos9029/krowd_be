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
        public async Task<string> CreateProjectWallet(ProjectWallet projectWalletDTO)
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
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @ProjectId, "
                    + "         0, "
                    + "         @WalletTypeId, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectWalletDTO.ProjectId, DbType.Guid);
                parameters.Add("WalletTypeId", projectWalletDTO.WalletTypeId, DbType.Guid);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", projectWalletDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", projectWalletDTO.UpdateBy, DbType.Guid);

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
        public async Task<List<ProjectWallet>> GetAllProjectWallets(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     ProjectId, "
                    + "                     WalletTypeId ASC ) AS Num, "
                    + "             * "
                    + "         FROM ProjectWallet "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         ProjectId, "
                    + "         Balance, "
                    + "         WalletTypeId, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<ProjectWallet>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM ProjectWallet WHERE IsDeleted = 0 ORDER BY ProjectId, WalletTypeId ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<ProjectWallet>(query)).ToList();
                }               
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
                    + "         WalletTypeId = @WalletTypeId, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectWalletDTO.ProjectId, DbType.Guid);
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
    }
}
