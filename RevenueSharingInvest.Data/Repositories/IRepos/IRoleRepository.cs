using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IRoleRepository
    {
        //CREATE
        public Task<string> CreateRole(Role roleDTO);

        //READ
        public Task<List<Role>> GetAllRoles();
        public Task<Role> GetRoleById(Guid roleId);
        public Task<Role> GetRoleByName(string roleName);
        public Task<Role> GetRoleByUserId(Guid userId);
        public Task<string> GetRoleNameByUserId(Guid userId);

        //UPDATE
        public Task<int> UpdateRole(Role roleDTO, Guid roleId);

        //DELETE
        public Task<int> DeleteRoleById(Guid roleId);
    }
}
