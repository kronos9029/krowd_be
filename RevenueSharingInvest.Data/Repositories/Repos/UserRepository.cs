﻿using Dapper;
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

        //CREATE
        public async Task<string> CreateUser(User userDTO)
        {
            try
            {
                var query = "INSERT INTO [User] ("
                    + "         BusinessId, "
                    + "         RoleId, "
                    + "         Description, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         PhoneNum, "
                    + "         Image, "
                    + "         IdCard, "
                    + "         Email, "
                    + "         Gender, "
                    + "         DateOfBirth, "
                    + "         TaxIdentificationNumber, "
                    + "         City, "
                    + "         District, "
                    + "         Ward, "
                    + "         Address, "                    
                    + "         BankName, "                    
                    + "         BankAccount, "                    
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @BusinessId, "
                    + "         @RoleId, "
                    + "         @Description, "
                    + "         @LastName, "
                    + "         @FirstName, "
                    + "         @PhoneNum, "
                    + "         @Image, "
                    + "         @IdCard, "
                    + "         @Email, "
                    + "         @Gender, "
                    + "         @DateOfBirth, "
                    + "         @TaxIdentificationNumber, "
                    + "         @City, "
                    + "         @District, "
                    + "         @Ward, "
                    + "         @Address, "
                    + "         @BankName, "
                    + "         @BankAccount, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", userDTO.BusinessId, DbType.Guid);
                parameters.Add("RoleId", userDTO.RoleId, DbType.Guid);
                parameters.Add("Description", userDTO.Description, DbType.String);
                parameters.Add("LastName", userDTO.LastName, DbType.String);
                parameters.Add("FirstName", userDTO.FirstName, DbType.String);
                parameters.Add("PhoneNum", userDTO.PhoneNum, DbType.String);
                parameters.Add("Image", userDTO.Image, DbType.String);
                parameters.Add("IdCard", userDTO.IdCard, DbType.String);
                parameters.Add("Email", userDTO.Email, DbType.String);
                parameters.Add("Gender", userDTO.Gender, DbType.String);
                parameters.Add("DateOfBirth", userDTO.DateOfBirth, DbType.String);
                parameters.Add("TaxIdentificationNumber", userDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("City", userDTO.City, DbType.String);
                parameters.Add("District", userDTO.District, DbType.String);
                parameters.Add("Ward", userDTO.Ward, DbType.String);
                parameters.Add("Address", userDTO.Address, DbType.String);
                parameters.Add("BankName", userDTO.BankName, DbType.String);
                parameters.Add("BankAccount", userDTO.BankAccount, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", userDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", userDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeleteUserById(Guid userId)//thiếu para UpdateBy đợi Authen
        {
            try
            {
                var query = "UPDATE [User] "
                    + "     SET "
                    + "         UpdateDate = @UpdateDate, "
                    //+ "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = 1 "
                    + "     WHERE "
                    + "         Id=@Id";
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                //parameters.Add("UpdateBy", userDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", userId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<User>> GetAllUsers(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     RoleId, "
                    + "                     FirstName ASC ) AS Num, "
                    + "             * "
                    + "         FROM [User] "
                    + "         WHERE "
                    + "             IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         BusinessId, "
                    + "         RoleId, "
                    + "         Description, "
                    + "         LastName, "
                    + "         FirstName, "
                    + "         PhoneNum, "
                    + "         Image, "
                    + "         IdCard, "
                    + "         Email, "
                    + "         Gender, "
                    + "         DateOfBirth, "
                    + "         TaxIdentificationNumber, "
                    + "         City, "
                    + "         District, "
                    + "         Ward, "
                    + "         Address, "
                    + "         BankName, "
                    + "         BankAccount, "
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
                    return (await connection.QueryAsync<User>(query, parameters)).ToList();
                }    
                else
                {
                    var query = "SELECT * FROM [User] WHERE IsDeleted = 0 ORDER BY RoleId, FirstName ASC";
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<User>(query)).ToList();
                }    
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<User> GetUserById(Guid userId)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", userId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }        
        
        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdateUser(User userDTO, Guid userId)
        {
            try
            {
                var query = "UPDATE [User] "
                    + "     SET "
                    + "         BusinessId = @BusinessId, "
                    + "         RoleId = @RoleId, "
                    + "         Description = @Description, "
                    + "         LastName = @LastName, "
                    + "         FirstName = @FirstName, "
                    + "         PhoneNum = @PhoneNum, "
                    + "         Image = @Image, "
                    + "         IdCard = @IdCard, "
                    + "         Email = @Email, "
                    + "         Gender = @Gender, "
                    + "         DateOfBirth = @DateOfBirth, "
                    + "         TaxIdentificationNumber = @TaxIdentificationNumber, "
                    + "         City = @City, "
                    + "         District = @District, "
                    + "         Ward = @Ward, "
                    + "         Address = @Address, "
                    + "         BankName = @BankName, "
                    + "         BankAccount = @BankAccount, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "         IsDeleted = @IsDeleted "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("BusinessId", userDTO.BusinessId, DbType.Guid);
                parameters.Add("RoleId", userDTO.RoleId, DbType.Guid);
                parameters.Add("Description", userDTO.Description, DbType.String);
                parameters.Add("LastName", userDTO.LastName, DbType.String);
                parameters.Add("FirstName", userDTO.FirstName, DbType.String);
                parameters.Add("PhoneNum", userDTO.PhoneNum, DbType.String);
                parameters.Add("Image", userDTO.Image, DbType.String);
                parameters.Add("IdCard", userDTO.IdCard, DbType.String);
                parameters.Add("Email", userDTO.Email, DbType.String);
                parameters.Add("Gender", userDTO.Gender, DbType.String);
                parameters.Add("DateOfBirth", userDTO.DateOfBirth, DbType.String);
                parameters.Add("TaxIdentificationNumber", userDTO.TaxIdentificationNumber, DbType.String);
                parameters.Add("City", userDTO.City, DbType.String);
                parameters.Add("District", userDTO.District, DbType.String);
                parameters.Add("Ward", userDTO.Ward, DbType.String);
                parameters.Add("Address", userDTO.Address, DbType.String);
                parameters.Add("BankName", userDTO.BankName, DbType.String);
                parameters.Add("BankAccount", userDTO.BankAccount, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", userDTO.UpdateBy, DbType.Guid);
                parameters.Add("IsDeleted", userDTO.IsDeleted, DbType.Boolean);
                parameters.Add("Id", userId, DbType.Guid);

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
        public async Task<int> ClearAllUserData()
        {
            try
            {
                var query = "DELETE FROM [User]";
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
