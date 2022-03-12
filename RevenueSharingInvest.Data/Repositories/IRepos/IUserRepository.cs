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
        public Task<User> GetUserByEmail(String userEmail);
        public Task<int> CreateUser(string email, DateTime createDate, string imageUrl, string BusinessId, string InvestorId, string RoleId);
    }
}
