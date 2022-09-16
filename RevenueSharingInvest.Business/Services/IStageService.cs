using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IStageService
    {
        //CREATE

        //READ
        public Task<AllStageDTO> GetAllStagesByProjectId(Guid projectId, ThisUserObj currentUser);
        public Task<GetStageDTO> GetStageById(Guid stageId, ThisUserObj currentUser);
        public Task<List<StageChartDTO>> GetStageChartByProjectId(Guid projectId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdateStage(UpdateStageDTO stageDTO, Guid stageId, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeleteStageById(Guid stageId);
    }
}
