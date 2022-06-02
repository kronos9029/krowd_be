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
        //CREATE
        public Task<string> CreateBusiness(Business businessDTO);

        //READ
        public Task<List<Business>> GetAllBusiness(int pageIndex, int pageSize);
        public Task<Business> GetBusinessById(Guid businesssId);

        //UPDATE
        public Task<int> UpdateBusiness(Business businessDTO, Guid businesssId);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businesssId);
        public Task<int> ClearAllBusinessData();
    }
}
