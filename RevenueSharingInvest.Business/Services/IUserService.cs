﻿using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IUserService
    {
        //public Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken);
        //public Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken);
        //CREATE
        public Task<IdDTO> CreateUser(CreateUserDTO userDTO, ThisUserObj currentUser);

        //READ
        //public Task<AllUserDTO> GetAllUsers(int pageIndex, int pageSize, string businessId, string projectId, string role, string status, ThisUserObj currentUser);
        public Task<AllUserDTO> GetAllAdmins(ThisUserObj currentUser);
        public Task<AllUserDTO> GetAllBusinesManagers(int pageIndex, int pageSize, Guid? businessId, string status, ThisUserObj currentUser);
        public Task<AllUserDTO> GetAllProjectManagers(int pageIndex, int pageSize, Guid? businessId, Guid? projectId, string status, ThisUserObj currentUser);
        public Task<AllUserDTO> GetAllInvestors(int pageIndex, int pageSize, Guid? projectId, string status, ThisUserObj currentUser);

        public Task<GetUserDTO> GetUserById(Guid userId);
        public Task<GetUserDTO> GetUserByEmail(String email);
        //public Task<List<User>> GetUserByBusinessId(Guid businessId);
        //public Task<List<User>> GetUserByRoleId(Guid businessId);
        public Task<GetUserDTO> BusinessManagerGetUserById(string businesId, Guid userId);
        public Task<GetUserDTO> ProjectManagerGetUserbyId(string managerId, Guid userId);
        public Task<string> GetProjectIdByManagerEmail(string email);
        public Task<IntegrateInfo> GetIntegrateInfoByEmailAndProjectId(string email, string projectId);

        //UPDATE
        public Task<int> UpdateUser(UpdateUserDTO userDTO, Guid userId, ThisUserObj currentUser);
        public Task<int> UpdateUserStatus(Guid userId, string status, ThisUserObj currentUser);
        public Task<int> UpdateUserEmail(Guid userId, string email, ThisUserObj currentUser);

        //DELETE
        //public Task<int> DeleteUserById(Guid userId);
    }
}
