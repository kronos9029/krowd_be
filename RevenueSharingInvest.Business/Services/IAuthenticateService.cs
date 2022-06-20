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
        public Task<bool> CheckRoleForUser(String userId, String requiredRole);
        public Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken);
        public Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken);
    }
}
