using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IRiskTypeRepository
    {
        //CREATE
        public Task<string> CreateRiskType(RiskType riskTypeDTO);

        //READ
        public Task<List<RiskType>> GetAllRiskTypes(int pageIndex, int pageSize);
        public Task<RiskType> GetRiskTypeById(Guid riskTypeId);
        public Task<List<RiskTypeObject>> GetRiskTypeInUse(Guid riskTypeId);

        //UPDATE
        public Task<int> UpdateRiskType(RiskType riskTypeDTO, Guid riskTypeId);

        //DELETE
        public Task<int> DeleteRiskTypeById(Guid riskTypeId);
        public Task<int> ClearAllRiskTypeData();
    }
}
