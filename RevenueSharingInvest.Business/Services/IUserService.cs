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
        public Task<IdDTO> CreateUser(UserDTO userDTO);

        //READ
        public Task<List<UserDTO>> GetAllUsers(int pageIndex, int pageSize);
        public Task<UserDTO> GetUserById(Guid userId);

        //UPDATE
        public Task<int> UpdateUser(UserDTO userDTO, Guid userId);

        //DELETE
        public Task<int> DeleteUserById(Guid userId);
        public Task<int> ClearAllUserData();
    }
}
