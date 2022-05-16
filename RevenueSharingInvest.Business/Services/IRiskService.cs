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
        public Task<int> CreateRisk(RiskDTO riskDTO);

        //READ
        public Task<List<RiskDTO>> GetAllRisks();
        public Task<RiskDTO> GetRiskById(Guid riskId);

        //UPDATE
        public Task<int> UpdateRisk(RiskDTO riskDTO, Guid riskId);

        //DELETE
        public Task<int> DeleteRiskById(Guid riskId);
    }
}
