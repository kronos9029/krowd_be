﻿using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs.GetAllDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPeriodRevenueHistoryService
    {
        //CREATE
        public Task<PeriodRevenueHistoryDTO> CreatePeriodRevenueHistory(CreatePeriodRevenueHistoryDTO createPeriodRevenueHistoryDTO, ThisUserObj currentUser);

        //READ
        public Task<AllPeriodRevenueHistoryDTO> GetAllPeriodRevenueHistories(int pageIndex, int pageSize, Guid projectId, ThisUserObj currentUser);
        public Task<PeriodRevenueHistoryDTO> GetPeriodRevenueHistoryById(Guid periodRevenueHistoryId);

        //UPDATE
        //public Task<int> UpdatePeriodRevenueHistory(PeriodRevenueHistoryDTO periodRevenueHistoryDTO, Guid periodRevenueHistoryId);

        //DELETE
        //public Task<int> DeletePeriodRevenueHistoryById(Guid periodRevenueHistoryId);
    }
}
