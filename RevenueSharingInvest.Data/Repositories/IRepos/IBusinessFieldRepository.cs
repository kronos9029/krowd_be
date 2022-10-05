using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IBusinessFieldRepository
    {
        //CREATE
        public Task<int> CreateBusinessField(Guid businessId, Guid fieldId, Guid creatorId);

        //READ
        public Task<List<BusinessField>> GetAllBusinessFields(int pageIndex, int pageSize);
        public Task<BusinessField> GetBusinessFieldById(Guid businessId, Guid fieldId);

        //UPDATE
        public Task<int> UpdateBusinessField(BusinessField businessFieldDTO, Guid businessId, Guid fieldId);

        //DELETE 
        public Task<int> DeleteBusinessFieldByBusinessId(Guid businessId);
    }
}
