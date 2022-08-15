using RevenueSharingInvest.API;
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
        public Task<IdDTO> CreateUser(CreateUserDTO userDTO);

        //READ
        public Task<AllUserDTO> GetAllUsers(int pageIndex, int pageSize, string businessId, string role, string status, string temp_field_role);
        public Task<GetUserDTO> GetUserById(Guid userId);
        public Task<GetUserDTO> GetUserByEmail(String email);
        //public Task<List<User>> GetUserByBusinessId(Guid businessId);
        //public Task<List<User>> GetUserByRoleId(Guid businessId);
        public Task<GetUserDTO> BusinessManagerGetUserById(string businesId, Guid userId);
        public Task<GetUserDTO> ProjectManagerGetUserbyId(string managerId, Guid userId);

        //UPDATE
        public Task<int> UpdateUser(UpdateUserDTO userDTO, Guid userId);

        //DELETE
        public Task<int> DeleteUserById(Guid userId);
        public Task<int> ClearAllUserData();
    }
}
