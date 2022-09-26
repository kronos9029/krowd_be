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
        public Task<List<RevenueSharingInvest.Data.Models.Entities.Business>> GetAllBusiness(int pageIndex, int pageSize, string status, string name, string orderBy, string order, string roleId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessById(Guid businesssId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByUserId(Guid userId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByProjectId(Guid projectId);
        public Task<RevenueSharingInvest.Data.Models.Entities.Business> GetBusinessByEmail(string email);
        public Task<int> CountBusiness(string status, string name, string roleId);

        //UPDATE
        public Task<int> UpdateBusiness(RevenueSharingInvest.Data.Models.Entities.Business businessDTO, Guid businesssId);
        public Task<int> UpdateBusinessStatus(Guid businessId, String status, Guid updaterId);
        public Task<int> UpdateBusinessImage(string url, Guid businessId);
        public Task<int> UpdateBusinessNumOfProject(Guid businessId);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businesssId);
        public Task<int> DeleteBusinessByBusinessId(Guid businessId);
        public Task<int> ClearAllBusinessData();
    }
}
