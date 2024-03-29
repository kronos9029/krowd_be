﻿using RevenueSharingInvest.API;
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
        public Task<string> CreateRepaymentStage(Guid projectId, ThisUserObj currentUser);

        //READ
        public Task<AllStageDTO> GetAllStagesByProjectId(Guid projectId, int pageIndex, int pageSize, string status, ThisUserObj currentUser);
        public Task<GetStageDTO> GetStageById(Guid stageId, ThisUserObj currentUser);
        public Task<List<StageChartDTO>> GetStageChartByProjectId(Guid projectId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdateStage(UpdateStageDTO stageDTO, Guid stageId, ThisUserObj currentUser);

        //DELETE
    }
}
