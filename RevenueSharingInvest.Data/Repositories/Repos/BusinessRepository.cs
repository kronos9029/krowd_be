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
    public class BusinessRepository : BaseRepository, IBusinessRepository
    {
        public BusinessRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        public async Task<int> CreateBusiness(Business businessDTO)
        {
            try
            {
                var query = "INSERT INTO Business (" 
                    + "         Name, " 
                    + "         PhoneNum,"    
                    + "         Image, " 
                    + "         Email, "
                    + "         Description, "
                    + "         TaxIdentificationNumber, "
                    + "         Address, "
                    + "         NumOfProject, "
                    + "         NumOfSuccessfulProject, "
                    + "         SuccessfulRate, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     VALUES ("
                    + "         @Name, "
                    + "         @PhoneNum, "
                    + "         @Image, "
                    + "         @Email, "
                    + "         @Description, "
                    + "         @TaxIdentificationNumber, "
                    + "         @Address, "
                    + "         @NumOfProject, "
                    + "         @NumOfSuccessfulProject, "
                    + "         @SuccessfulRate, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0)";
                var parameters = new DynamicParameters();
                parameters.Add("Name", businessDTO.Name, DbType.String);
                parameters.Add("PhoneNum", businessDTO.PhoneNum, DbType.String);
                parameters.Add("Image", businessDTO.Image, DbType.String);
                parameters.Add("Email", businessDTO.Email, DbType.String);
                parameters.Add("Description", businessDTO.Description, DbType.String);
                parameters.Add("TaxIdentificationNumber", businessDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("Address", businessDTO.Address, DbType.String);
                parameters.Add("NumOfProject", businessDTO.NumOfProject, DbType.Int16);
                parameters.Add("NumOfSuccessfulProject", businessDTO.NumOfSuccessfulProject, DbType.Int16);
                parameters.Add("SuccessfulRate", businessDTO.SuccessfulRate, DbType.Double);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", businessDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteBusinessById(Guid businesssId)
        {
            try
            {
                var query = "UPDATE Business "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", areaDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", businesssId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Business>> GetAllBusiness()
        {
            try
            {
                string query = "SELECT * FROM Business";
                using var connection = CreateConnection();
                return (await connection.QueryAsync<Business>(query)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<Business> GetBusinessById(Guid businesssId)
        {
            try
            {
                var query = "SELECT * FROM Business WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", businesssId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Business>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(Business businessDTO, Guid businesssId)
        {
            try
            {
                var query = "UPDATE Business "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         PhoneNum = @PhoneNum, "
                    + "         Image = @Image, "
                    + "         Email = @Email, "
                    + "         Description = @Description, "
                    + "         TaxIdentificationNumber = @TaxIdentificationNumber, "
                    + "         Address = @Address, "
                    + "         NumOfProject = @NumOfProject, "
                    + "         NumOfSuccessfulProject = @NumOfSuccessfulProject, "
                    + "         SuccessfulRate = @SuccessfulRate, "
                    + "         CreateDate = @CreateDate, "
                    + "         CreateBy = @CreateBy, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted"
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", businessDTO.Name, DbType.String);
                parameters.Add("PhoneNum", businessDTO.PhoneNum, DbType.String);
                parameters.Add("Image", businessDTO.Image, DbType.String);
                parameters.Add("Email", businessDTO.Email, DbType.String);
                parameters.Add("Description", businessDTO.Description, DbType.String);
                parameters.Add("TaxIdentificationNumber", businessDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("Address", businessDTO.Address, DbType.String);
                parameters.Add("NumOfProject", businessDTO.NumOfProject, DbType.Int16);
                parameters.Add("NumOfSuccessfulProject", businessDTO.NumOfSuccessfulProject, DbType.Int16);
                parameters.Add("SuccessfulRate", businessDTO.SuccessfulRate, DbType.Double);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", businessDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", businessDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", businesssId, DbType.Guid);

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
