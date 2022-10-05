using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IRiskTypeService
    {
        //CREATE
        public Task<IdDTO> CreateRiskType(RiskTypeDTO riskTypeDTO);

        //READ
        public Task<List<RiskTypeDTO>> GetAllRiskTypes(int pageIndex, int pageSize);
        public Task<RiskTypeDTO> GetRiskTypeById(Guid riskTypeId);

        //UPDATE
        public Task<int> UpdateRiskType(RiskTypeDTO riskTypeDTO, Guid riskTypeId);

        //DELETE
        public Task<int> DeleteRiskTypeById(Guid riskTypeId);
    }
}
