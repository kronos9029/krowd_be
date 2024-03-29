﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IWithdrawRequestRepository
    {
        //CREATE
        public Task<string> CreateWithdrawRequest(WithdrawRequest request);

        //READ
        public Task<WithdrawRequest> GetWithdrawRequestByRequestIdAndUserId(Guid requestId, Guid userId);
        public Task<List<WithdrawRequest>> GetWithdrawRequestByUserId(Guid userId);
        public Task<List<WithdrawRequest>> GetAllWithdrawRequest(int pageIndex, int pageSize, string userId, string filter);
        public Task<int> CountAllWithdrawRequest(string userId, string filter);
        public Task<WithdrawRequest> GetWithdrawRequestByRequestId(Guid requestId);

        //UPDATE
        public Task<int> AdminApproveWithdrawRequest(Guid userId, Guid requestId, string receipt);
        public Task<int> UserApproveWithdrawRequest(Guid userId, Guid requestId);
        public Task<int> AdminRejectWithdrawRequest(Guid userId, Guid requestId, string RefusalReason);
        public Task<int> ReportWithdrawRequest(Guid userId, Guid requestId, string reportMessage);

        //DELETE

    }
}
