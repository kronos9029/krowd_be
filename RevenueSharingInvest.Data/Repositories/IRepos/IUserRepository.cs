using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<List<User>> GetAllUsers(int pageIndex, int pageSize, string businessId, string projectManagerId, string roleId, string status, string thisUserRoleId);
        public Task<User> GetUserById(Guid userId);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetBusinessManagerByBusinessId(Guid businessId);
        public Task<User> GetProjectManagerByProjectId(Guid projectId);
        public Task<List<User>> GetProjectMembers(Guid projectId);
        public Task<List<User>> GetUserByBusinessId(Guid businessId);
        public Task<int> CountUser(string businessId, string projectManagerId, string roleId, string status, string thisUserRoleId);
        public Task<User> BusinessManagerGetUserById(Guid businessId, Guid userid);
        public Task<User> ProjectManagerGetUserbyId(Guid managerId, Guid id);

        //UPDATE
        public Task<int> UpdateUser(User userDTO, Guid userId);
        public Task<int> UpdateBusinessIdForBuM(Guid? businessId, Guid businesManagerId);
        public Task<int> UpdateUserImage(string url, Guid userId);
        public Task<int> UpdateUserStatus(Guid userId, string status, Guid updaterId);
        public Task<int> UpdateUserEmail(Guid userId, string email, Guid updaterId);

        //DELETE
        public Task<int> DeleteUserById(Guid userId);
        public Task<int> ClearAllUserData();
    }
}
