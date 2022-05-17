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
    public class WalletTypeRepository : BaseRepository, IWalletTypeRepository
    {
        public WalletTypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateWalletType(WalletType walletTypeDTO)
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
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @Description, "
                    + "         @Mode, "
                    + "         @Type, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", walletTypeDTO.Name, DbType.String);
                parameters.Add("Description", walletTypeDTO.Description, DbType.String);
                parameters.Add("Mode", walletTypeDTO.Mode, DbType.String);
                parameters.Add("Type", walletTypeDTO.Type, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", walletTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", walletTypeDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteWalletTypeById(Guid walletTypeId)//thiếu para UpdateBy
        {
            try
            {
                var query = "UPDATE WalletType "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", walletTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", walletTypeId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<WalletType>> GetAllWalletTypes()
        {
            try
            {
                string query = "SELECT * FROM WalletType";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<WalletType>(query)).ToList();
            }
            catch (Exception e)
            {
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
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", walletTypeDTO.Name, DbType.String);
                parameters.Add("Description", walletTypeDTO.Description, DbType.String);
                parameters.Add("Mode", walletTypeDTO.Mode, DbType.String);
                parameters.Add("Type", walletTypeDTO.Type, DbType.String);
                parameters.Add("CreateDate", walletTypeDTO.CreateDate, DbType.DateTime);
                parameters.Add("CreateBy", walletTypeDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", walletTypeDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", walletTypeDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", walletTypeId, DbType.Guid);

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
