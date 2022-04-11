using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IBusinessRepository
    {
        Task<Business> GetBusinessById(Guid businesssId);

        Task<List<Business>> GetAllBusiness();
        Task<int> CreateBusiness(Business newBusiness);
    }
}
