using RevenueSharingInvest.API;
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
        public Task<GetWithdrawRequestDTO> CreateWithdrawRequest(WithdrawRequestDTO request, ThisUserObj currentUser);

        //READ
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestByRequestIdAndUserId(string requestId, string userId);
        public Task<List<GetWithdrawRequestDTO>> GetWithdrawRequestByUserId(string userId);
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id);        
        public Task<AllWithdrawRequestDTO> GetAllWithdrawRequest(int pageIndex, int pageSize, string userId, string filter);
        public Task<dynamic> AdminApproveWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string receipt);

        //UPDATE
        public Task<dynamic> ApproveWithdrawRequest(string userId, GetWithdrawRequestDTO request);

        public Task<dynamic> AdminRejectWithdrawRequest(ThisUserObj userId, GetWithdrawRequestDTO request, string RefusalReason);

        public Task<dynamic> ReportWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string reportMessage);
        public Task<dynamic> AdminResponeToWithdrawRequest(ThisUserObj currentUser, GetWithdrawRequestDTO request, string receipt);

    }
}
