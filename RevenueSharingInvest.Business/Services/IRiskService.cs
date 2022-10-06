using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RevenueSharingInvest.Business.Services
{
    public interface IRiskService
    {
        //CREATE
        public Task<IdDTO> CreateRisk(RiskDTO riskDTO);

        //READ
        public Task<List<RiskDTO>> GetAllRisks(int pageIndex, int pageSize);
        public Task<RiskDTO> GetRiskById(Guid riskId, ThisUserObj currentUser);
        public Task<List<RiskDTO>> GetAllRisksByBusinessId(int pageIndex, int pageSize, string businessId);
        //UPDATE
        public Task<int> UpdateRisk(RiskDTO riskDTO, Guid riskId, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeleteRiskById(Guid riskId, ThisUserObj currentUser);
    }
}
