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
        public Task<int> CreateStage(StageDTO stageDTO);

        //READ
        public Task<List<StageDTO>> GetAllStages();
        public Task<StageDTO> GetStageById(Guid stageId);

        //UPDATE
        public Task<int> UpdateStage(StageDTO stageDTO, Guid stageId);

        //DELETE
        public Task<int> DeleteStageById(Guid stageId);
    }
}
