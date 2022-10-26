using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IRoleService
    {
        //CREATE
        public Task<IdDTO> CreateRole(RoleDTO roleDTO);

        //READ
        public Task<List<RoleDTO>> GetAllRoles();
        public Task<RoleDTO> GetRoleById(Guid roleId);
        public Task<string> GetRoleNameByUserId(string userId);

        //UPDATE
        public Task<int> UpdateRole(RoleDTO roleDTO, Guid roleId);

        //DELETE
        public Task<int> DeleteRoleById(Guid roleId);
    }
}
