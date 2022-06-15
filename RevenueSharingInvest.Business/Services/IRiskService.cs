using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IRiskService
    {
        //CREATE
        public Task<IdDTO> CreateRisk(RiskDTO riskDTO);

        //READ
        public Task<List<RiskDTO>> GetAllRisks(int pageIndex, int pageSize);
        public Task<RiskDTO> GetRiskById(Guid riskId);

        //UPDATE
        public Task<int> UpdateRisk(RiskDTO riskDTO, Guid riskId);

        //DELETE
        public Task<int> DeleteRiskById(Guid riskId);
        public Task<int> ClearAllRiskData();
    }
}
