﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IUserRepository
    {
        //CREATE
        public Task<string> CreateUser(User userDTO);

        //READ
        public Task<List<User>> GetAllUsers(int pageIndex, int pageSize);
        public Task<User> GetUserById(Guid userId);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetBusinessManagerByBusinessId(Guid businessId);

        //UPDATE
        public Task<int> UpdateUser(User userDTO, Guid userId);

        //DELETE
        public Task<int> DeleteUserById(Guid userId);
        public Task<int> ClearAllUserData();
    }
}
