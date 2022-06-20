using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IAuthenticateService
    {
        public Task<bool> CheckRoleForAction(String userId, String requiredRole);
        public Task<bool> CheckIdForAction(String userId, Guid projectId);
        public Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken);
        public Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken);
    }
}
