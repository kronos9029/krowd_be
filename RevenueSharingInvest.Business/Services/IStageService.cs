using RevenueSharingInvest.Data.Models.DTOs;
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
        public Task<IdDTO> CreateStage(StageDTO stageDTO);

        //READ
        public Task<List<StageDTO>> GetAllStages(int pageIndex, int pageSize);
        public Task<StageDTO> GetStageById(Guid stageId);

        //UPDATE
        public Task<int> UpdateStage(StageDTO stageDTO, Guid stageId);

        //DELETE
        public Task<int> DeleteStageById(Guid stageId);
        public Task<int> ClearAllStageData();
    }
}
