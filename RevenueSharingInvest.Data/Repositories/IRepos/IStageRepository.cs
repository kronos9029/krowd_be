using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IStageRepository
    {
        //CREATE
        public Task<string> CreateStage(Stage stageDTO);

        //READ
        public Task<List<Stage>> GetAllStagesByProjectId(Guid projectId);
        public Task<Stage> GetStageById(Guid stageId);

        //UPDATE
        public Task<int> UpdateStage(Stage stageDTO, Guid stageId);

        //DELETE
        public Task<int> DeleteStageById(Guid stageId);
        public Task<int> DeleteStageByProjectId(Guid projectId);
        public Task<int> ClearAllStageData();
    }
}
