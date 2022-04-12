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
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            try
            {
                string query = "SELECT * FROM User WHERE Email=@Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", userEmail, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }
            
        }

        public async Task<int> CreateInvestorUser(User newUser)
        {
            try
            {
                var query = "INSERT INTO User (Email, CreateDate, Image, BusinessId, InvestorId, RoleId) VALUES (@Email, @CreateDate, @Image, @InvestorId, @RoleId)";
                var parameters = new DynamicParameters();
                parameters.Add("Email", newUser.Email, DbType.String);
                parameters.Add("CreateDate", newUser.CreateDate, DbType.DateTime);
                parameters.Add("Image", newUser.Image, DbType.String);
                parameters.Add("InvestorId", newUser.InvestorId, DbType.Guid);
                parameters.Add("RoleId", newUser.RoleId, DbType.Guid);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            } catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task<int> CreateBusinessUser(User newUser)
        {
            try
            {
                var query = "INSERT INTO User (Email, CreateDate, Image, BusinessId, RoleId, Description, LastName, FirstName, PhoneNum, IdCard, Gender, DateOfBirth, TaxIdentificationNumber, City, District, Ward, Address, Bank, BankAccount, CreateBy, IsDeleted) " +
                    "VALUES (@Email, @CreateDate, @Image, @BusinessId, @RoleId, @Description, @LastName, @FirstName, @PhoneNum, @IdCard, @Gender, @DateOfBirth, @TaxIdentificationNumber, @City, @District, @Ward, @Address, @Bank, @BankAccount, @CreateBy, 0)";
                var parameters = new DynamicParameters();
                parameters.Add("Email", newUser.Email, DbType.String);
                parameters.Add("CreateDate", newUser.CreateDate, DbType.DateTime);
                parameters.Add("Image", newUser.Image, DbType.String);
                parameters.Add("BusinessId", newUser.BusinessId, DbType.Guid);
                parameters.Add("RoleId", newUser.RoleId, DbType.Guid);
                parameters.Add("Description", newUser.RoleId, DbType.String);
                parameters.Add("LastName", newUser.RoleId, DbType.String);
                parameters.Add("FirstName", newUser.RoleId, DbType.String);
                parameters.Add("PhoneNum", newUser.RoleId, DbType.String);
                parameters.Add("IdCard", newUser.RoleId, DbType.String);
                parameters.Add("Gender", newUser.RoleId, DbType.String);
                parameters.Add("DateOfBirth", newUser.RoleId, DbType.DateTime);
                parameters.Add("TaxIdentificationNumber", newUser.RoleId, DbType.String);
                parameters.Add("City", newUser.RoleId, DbType.String);
                parameters.Add("District", newUser.RoleId, DbType.String);
                parameters.Add("Ward", newUser.RoleId, DbType.String);
                parameters.Add("Address", newUser.RoleId, DbType.String);
                parameters.Add("Bank", newUser.RoleId, DbType.String);
                parameters.Add("BankAccount", newUser.RoleId, DbType.String);
                parameters.Add("CreateBy", newUser.RoleId, DbType.String);
                parameters.Add("IsDeleted", newUser.RoleId, DbType.Binary);

                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
