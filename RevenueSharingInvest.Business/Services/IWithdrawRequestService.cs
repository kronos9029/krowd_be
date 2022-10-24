using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IWithdrawRequestService
    {
        public Task<string> CreateInvestorWithdrawRequest(InvestorWithdrawRequest request, ThisUserObj currentUser);

        public Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, string requestId, double amount);

        public Task<dynamic> InvestorApproveWithdrawRequest(string userId, string requestId);

        public Task<dynamic> AdminRejectWithdrawRequest(string userId, string requestId, string RefusalReason);

        public Task<dynamic> InvestorReportWithdrawRequest(string userId, string requestId, string description);
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestByRequestIdAndUserId(string requestId, string userId);
        public Task<List<GetWithdrawRequestDTO>> GetWithdrawRequestByUserId(string userId);
        public Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, string requestId);

    }
}
