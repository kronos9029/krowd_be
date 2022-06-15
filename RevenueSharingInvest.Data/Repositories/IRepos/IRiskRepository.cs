using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IRiskRepository
    {
        //CREATE
        public Task<string> CreateRisk(Risk riskDTO);

        //READ
        public Task<List<Risk>> GetAllRisks(int pageIndex, int pageSize);
        public Task<Risk> GetRiskById(Guid riskId);

        //UPDATE
        public Task<int> UpdateRisk(Risk riskDTO, Guid riskId);

        //DELETE
        public Task<int> DeleteRiskById(Guid riskId);
        public Task<int> ClearAllRiskData();
    }
}
