﻿using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<List<Stage>> GetAllStagesByProjectId(Guid projectId, int pageIndex, int pagesize);
        public Task<Stage> GetStageById(Guid stageId);
        public Task<Stage> GetStageByProjectIdAndDate(Guid projectId, string date);
        public Task<Stage> GetLastStageByProjectId(Guid projectId);
        public Task<int> CountAllStagesByProjectId(Guid projectId);

        //UPDATE
        public Task<int> UpdateStage(Stage stageDTO, Guid stageId);

        //DELETE
        public Task<int> DeleteStageByProjectId(Guid projectId);
    }
}
