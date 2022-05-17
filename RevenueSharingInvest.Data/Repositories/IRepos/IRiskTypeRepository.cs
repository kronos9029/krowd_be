using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<int> CreateRiskType(RiskType riskTypeDTO);

        //READ
        public Task<List<RiskType>> GetAllRiskTypes();
        public Task<RiskType> GetRiskTypeById(Guid riskTypeId);

        //UPDATE
        public Task<int> UpdateRiskType(RiskType riskTypeDTO, Guid riskTypeId);

        //DELETE
        public Task<int> DeleteRiskTypeById(Guid riskTypeId);
    }
}
