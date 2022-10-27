using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IWithdrawRequestRepository
    {
        public Task<string> CreateWithdrawRequest(WithdrawRequest request);
        public Task<int> AdminApproveWithdrawRequest(Guid userId, Guid requestId);
        public Task<int> InvestorApproveWithdrawRequest(Guid userId, Guid requestId);
        public Task<int> AdminRejectWithdrawRequest(Guid userId, Guid requestId, string RefusalReason);
        public Task<int> InvestorReportWithdrawRequest(Guid userId, Guid requestId, string description);
        public Task<WithdrawRequest> GetWithdrawRequestByRequestIdAndUserId(Guid requestId, Guid userId);
        public Task<List<WithdrawRequest>> GetWithdrawRequestByUserId(Guid userId);
        public Task<List<WithdrawRequest>> AdminGetAllWithdrawRequest();
    }
}
