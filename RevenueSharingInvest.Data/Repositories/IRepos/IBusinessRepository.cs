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
        public Task<string> CreateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO);

        //READ
        public Task<List<RevenueSharingInvest.Data.Models.Entities.Business>> GetAllBusiness(int pageIndex, int pageSize, string? orderBy, string? order, string role);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessById(Guid businesssId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByUserId(Guid userId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByProjectId(Guid projectId);
        public Task<int> CountBusiness(string role);

        //UPDATE
        public Task<int> UpdateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO, Guid businesssId);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businesssId);
        public void DeleteBusinessByBusinessId(Guid businessId);
        public Task<int> ClearAllBusinessData();
    }
}
