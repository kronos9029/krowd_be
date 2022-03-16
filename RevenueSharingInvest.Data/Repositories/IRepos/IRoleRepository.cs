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
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleById(Guid roleId);
    }
}
