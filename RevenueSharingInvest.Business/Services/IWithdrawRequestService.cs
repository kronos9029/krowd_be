﻿using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IWithdrawRequestService
    {
        //CREATE
        public Task<GetWithdrawRequestDTO> CreateInvestorWithdrawRequest(WithdrawRequestDTO request, ThisUserObj currentUser);

        //READ
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestByRequestIdAndUserId(string requestId, string userId);
        public Task<List<GetWithdrawRequestDTO>> GetWithdrawRequestByUserId(string userId);
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id);        
        public Task<AllWithdrawRequestDTO> GetAllWithdrawRequest(int pageIndex, int pageSize, string userId, string filter);

        //UPDATE
        public Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, string requestId, string receipt);
        public Task<dynamic> ApproveWithdrawRequest(string userId, string requestId);
        public Task<dynamic> ReportWithdrawRequest(string userId, string requestId, string reportMessage);
        public Task<dynamic> AdminRejectWithdrawRequest(string userId, string requestId, string RefusalReason);
        public Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, string requestId, string receipt);

        //DELETE

    }
}
